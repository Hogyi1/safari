using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class TerrainController : MonoBehaviour
{
    public Terrain terrain;

    [SerializeField]
    float restoreSpeed = 5f;
    [SerializeField]
    float blendingStrength = 5f;
    [SerializeField]
    int blendingArea = 5;


    public GameObject CurrentObject;

    TerrainData terrainData;

    // Terrain világ koordinátában
    Vector3 terrainWorldPos;

    // Building világ koordinátái
    Vector3 buildingPos;

    // Building sarkok világ koordinátái
    Bounds bounds;

    // Terrain szélesség hosszúság világ koordinátában 100-600-100
    Vector3 terrainSize;

    // Normalizált magasság ahol a building alja van
    float targetHeightInHeightMap;

    // Resolution 513
    int res;

    // xStart, yStart, Index - heightMap
    private Dictionary<Vector3Int, float[,]> storedHeights = new Dictionary<Vector3Int, float[,]>();

    public void AdjustTerrainToBuilding(GameObject building, int buildingIndex)
    {
        if (terrain == null) return;

        // Beállítja a közös változókat
        SetCurrentData(building);

        // Elmenti az eredeti magasságokat, hogyha visszaakarnánk állítani
        SaveOriginalHeightMap(buildingIndex);

        // felhúzza a domborzatot az épület aljáig
        FlattenTerrainUnderBuilding();

        // Eleggyengeti a körzetében
        FlattenTerrainNearbyBuilding();
    }

    private void SetCurrentData(GameObject building)
    {
        // Terrain és building adatainak beállítása
        terrainData = terrain.terrainData;

        // Terrain világ koordinátában
        terrainWorldPos = terrain.transform.position;

        // Terrain szélesség hosszúság világ koordinátában 100-600-100
        terrainSize = terrainData.size;

        // Resolution 513
        res = terrainData.heightmapResolution;

        // Building világ koordinátái
        buildingPos = building.transform.position;

        // Building sarkok világ koordinátái
        bounds = building.GetComponentInChildren<Renderer>().bounds;

        // A building aljának koordinátája átváltva - normalizálva heightmapre
        targetHeightInHeightMap = (bounds.min.y - terrainWorldPos.y) / terrainData.size.y;
    }

    private void FlattenTerrainUnderBuilding()
    {

        // A building relatív koordinátája a terrainhez képest
        Vector3 relativeCorner = bounds.min - terrainWorldPos; // min x,z → bal alsó első - szemből

        // Terrain koordináta a világ koordinátából átváltva
        int zStart = Mathf.FloorToInt((relativeCorner.z / terrainSize.z) * res);
        int xStart = Mathf.CeilToInt((relativeCorner.x / terrainSize.x) * res);

        // Terrain szélesség a világ koordinátából átváltva
        int depth = Mathf.CeilToInt((bounds.size.z / terrainSize.z) * res); // - Z
        int width = Mathf.CeilToInt((bounds.size.x / terrainSize.x) * res); // - X

        float[,] newHeights = new float[depth, width];

        // Az új heightmap létrehozása
        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                newHeights[z, x] = targetHeightInHeightMap;
            }
        }

        terrainData.SetHeights(xStart, zStart, newHeights);

    }

    private void FlattenTerrainNearbyBuilding()
    {
        // Offset az épület nagysága szerint
        Vector3 offset = bounds.center - bounds.min;

        // Koordináta amihez nézze az eltolást
        Vector3 offsetPosition = new Vector3(bounds.min.x - offset.x, bounds.min.y, bounds.min.z - offset.z);

        // A hosszúság eltolással
        float offsetDepth = bounds.size.z + 2 * offset.z;
        float offsetWidth = bounds.size.x + 2 * offset.x;

        // A building relatív koordinátája a terrainhez képest
        Vector3 relativeCorner = offsetPosition - terrainWorldPos; // min x,z → bal alsó első - szemből

        // Terrain koordináta a világ koordinátából átváltva
        int xStart = Mathf.CeilToInt((relativeCorner.x / terrainSize.x) * res);
        int zStart = Mathf.FloorToInt((relativeCorner.z / terrainSize.z) * res);

        // Terrain szélesség a világ koordinátából átváltva
        int width = Mathf.CeilToInt((offsetWidth / terrainSize.x) * res); // - X
        int depth = Mathf.CeilToInt((offsetDepth / terrainSize.z) * res); // - Z

        float[,] currentHeightMap = terrainData.GetHeights(xStart, zStart, width, depth);
        float[,] newHeights = new float[depth, width];

        // Legtávolabbi távolság a pontok és a ház között
        float maxDistance = Vector3.Distance(bounds.min, relativeCorner) + 0.1f;
        Debug.Log(maxDistance + " Maxdistance ");
        Debug.Log("The offset is: " + relativeCorner);


        // Végigmegyünk a lekért heightmap szelet minden Z (sor) elemén
        for (int z = 0; z < depth; z++)
        {
            // Végigmegyünk minden X (oszlop) elemén a Z sorban
            for (int x = 0; x < width; x++)
            {
                // A heightmap (x, z) indexhez tartozó világkoordináta kiszámítása X tengelyen
                float worldX = terrainWorldPos.x + ((float)(xStart + x) / res) * terrainSize.x;
                float worldZ = terrainWorldPos.z + ((float)(zStart + z) / res) * terrainSize.z;

                // Világkoordináta a jelelnlegi pontban
                Vector3 worldPoint = new Vector3(worldX, buildingPos.y, worldZ);

                float distance = Vector3.Distance(worldPoint, bounds.ClosestPoint(worldPoint));

                // Foglalt-e már a hely
                bool canDeform = BuildingManager.Instance.IsEmpty(worldPoint);

                // A távolságot normalizáljuk 0 és 1 közé (1 = legmesszebb, 0 = mellette van)
                float normalizedDistance = canDeform ? Mathf.Clamp01(distance / maxDistance) : 1f;

                // Az adott heightmap pont eredeti magassága
                float originalHeight = currentHeightMap[z, x];

                // Interpoláció az épület alja és a jelenlegi között
                // Minél közelebb van az épülethez a pont, annál jobban eléri a célmagasságot
                float interpolatedHeight = Mathf.Lerp(targetHeightInHeightMap, originalHeight, normalizedDistance);

                // Az interpolált magasság beállítása
                newHeights[z, x] = interpolatedHeight;
            }
        }

        // Beállítás az új heightmapre
        terrainData.SetHeights(xStart, zStart, newHeights);

    }

    private void SaveOriginalHeightMap(int index)
    {
        // Offset az épület nagysága szerint
        Vector3 offset = bounds.center - bounds.min;

        // Koordináta amihez nézze az eltolást
        Vector3 offsetPosition = new Vector3(bounds.min.x - offset.x, bounds.min.y, bounds.min.z - offset.z);

        // A hosszúság eltolással
        float offsetDepth = bounds.size.z + 2 * offset.z;
        float offsetWidth = bounds.size.x + 2 * offset.x;

        // A building relatív koordinátája a terrainhez képest
        Vector3 relativeCorner = offsetPosition - terrainWorldPos; // min x,z → bal alsó első - szemből
        Vector3 relativeMax = bounds.max - terrainWorldPos; // max x,z → jobb felső hátsó - szemből

        // Terrain koordináta a világ koordinátából átváltva
        int xStart = Mathf.CeilToInt((relativeCorner.x / terrainSize.x) * res);
        int zStart = Mathf.FloorToInt((relativeCorner.z / terrainSize.z) * res);

        // Terrain szélesség a világ koordinátából átváltva
        int width = Mathf.CeilToInt((offsetWidth / terrainSize.x) * res); // - X
        int depth = Mathf.CeilToInt((offsetDepth / terrainSize.z) * res); // - Z

        // Az eredeti heightmap
        float[,] originalHeightMap = terrainData.GetHeights(xStart, zStart, depth, width);

        // A kulcs, xStart, zStart és a buildingIndex-ből áll
        Vector3Int SaveKey = new Vector3Int(xStart, zStart, index);
        storedHeights[SaveKey] = originalHeightMap;
    }

    public void RestoreTerrain(int buildingIndex)
    {
        Vector3Int RestoreKey = Vector3Int.zero;
        float[,] heightMap = null;

        foreach (Vector3Int key in storedHeights.Keys)
        {
            if (key.z == buildingIndex)
            {
                heightMap = storedHeights[key];
                RestoreKey = key;
                storedHeights.Remove(key);
                break;
            }
        }

        if (RestoreKey == Vector3Int.zero)
        {
            Debug.LogError("Ilyen ID-val rendelkező építmén nem létezik a terrainen: " + buildingIndex);
            return;
        }

        StartCoroutine(SmoothRestore(RestoreKey.x, RestoreKey.y, heightMap));
        Debug.Log("A domborzat visszaállítása.");
    }

    private IEnumerator SmoothRestore(int xStart, int zStart, float[,] originalHeightMap)
    {
        if (terrain == null) yield return null;

        int width = originalHeightMap.GetLength(1);  // - X 
        int depth = originalHeightMap.GetLength(0);  // - Z 

        float progress = 0.0f;

        while (progress < 1f)
        {
            progress += Time.unscaledDeltaTime * restoreSpeed;

            float[,] currentHeights = terrainData.GetHeights(xStart, zStart, width, depth);

            float[,] newHeights = new float[depth, width];

            for (int z = 0; z < depth; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    // A heightmap (x, z) indexhez tartozó világkoordináta kiszámítása X tengelyen
                    float worldX = terrainWorldPos.x + ((float)(xStart + x) / res) * terrainSize.x;
                    float worldZ = terrainWorldPos.z + ((float)(zStart + z) / res) * terrainSize.z;

                    // Világkoordináta a jelelnlegi pontban
                    Vector3 worldPoint = new Vector3(worldX, buildingPos.y, worldZ);

                    // Foglalt-e már a hely
                    bool canDeform = BuildingManager.Instance.IsEmpty(worldPoint);

                    float interpolation = canDeform ? progress : 0f;

                    newHeights[z, x] = Mathf.Lerp(currentHeights[z, x], originalHeightMap[z, x], interpolation);
                }
            }

            terrainData.SetHeights(xStart, zStart, newHeights);
            yield return null;
        }
    }



}

