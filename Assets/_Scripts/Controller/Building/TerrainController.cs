using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using System;
using System.Linq;

public class TerrainController : MonoBehaviour
{
    public Terrain terrain;

    [SerializeField]
    private float restoreSpeed = 5f;
    [SerializeField]
    private float blendingStrength = 5f;
    [SerializeField]
    private float blendingArea = 1f;


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

    // private byte[,] heights = new byte[513,513]; a valtoztatott koordiinatak 

    // private Dictionary<Vector3Int, int>
    public void AdjustTerrainToBuilding(GameObject building, int buildingIndex, bool saveOriginal)
    {
        if (terrain == null) return;

        // Beállítja a közös változókat
        SetCurrentData(building);

        // Elmenti az eredeti magasságokat, hogyha visszaakarnánk állítani
        SaveOriginalHeightMap(buildingIndex, saveOriginal);

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

        // A jelenlegire beállítom
        CurrentObject = building;

        // Building világ koordinátái
        buildingPos = building.transform.position;


        // Building tényleges alja
        try
        {
            GameObject bottom = CurrentObject.GetComponentsInChildren<GameObject>().FirstOrDefault(t => t.name.ToUpper() == "FLOOR");
            bounds = bottom.GetComponent<Renderer>().bounds;
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex);

            // Ha nincs Floor akkor az alapot használjuk
            bounds = building.GetComponentInChildren<Renderer>().bounds;
            Debug.LogWarning("Floor nem található, Renderer bounds lesz használva!");
        }

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
        Vector3 offset = new Vector3(blendingArea, 0, blendingArea);

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
        float maxDistance2 = Vector3.Distance(bounds.min, relativeCorner);
        float maxDistance = Vector3.Distance(bounds.min, new Vector3(bounds.min.x - offset.x, bounds.min.y, bounds.min.z));
        Debug.Log(maxDistance + " Maxdistance ");
        Debug.Log(maxDistance2 + " Maxdistance2 ");
        Debug.Log("The offset is: " + relativeCorner);


        // Végigmegyünk a lekért heightmap szelet minden Z (sor) elemén
        for (int z = 0; z < depth; z++)
        {
            // Végigmegyünk minden X (oszlop) elemén a Z sorban
            for (int x = 0; x < width; x++)
            {
                // A heightmap jelenlegi pontja a világ koordinátában
                float worldX = terrainWorldPos.x + ((float)(xStart + x) / res) * terrainSize.x;
                float worldZ = terrainWorldPos.z + ((float)(zStart + z) / res) * terrainSize.z;

                // Világkoordináta a jelelnlegi pontban
                Vector3 worldPoint = new Vector3(worldX, buildingPos.y, worldZ);

                float distance = Vector3.Distance(worldPoint, bounds.ClosestPoint(worldPoint));

                // Foglalt-e már a hely
                GameObject canDeform = BuildingManager.Instance.IsEmpty(worldPoint);


                // A távolságot normalizáljuk 0 és 1 közé (1 = legmesszebb, 0 = mellette van)
                float normalizedDistance = (canDeform == null || canDeform == CurrentObject) ? Mathf.Clamp01(distance / maxDistance) : 1f;

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

    private void SaveOriginalHeightMap(int index, bool saveOriginal)
    {

        // Offset az épület nagysága szerint
        Vector3 offset = new Vector3(blendingArea, 0, blendingArea);

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

        if (saveOriginal)
        {
            storedHeights[SaveKey] = originalHeightMap;
        }
        else
        {
            storedHeights.Remove(SaveKey);
        }
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
            Debug.LogError("Ilyen ID-val rendelkező építmény nem létezik a domborzaton: " + buildingIndex);
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

        // Visszaállításra szorolú épületek
        HashSet<GameObject> restore = new();

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
                    GameObject building = BuildingManager.Instance.IsEmpty(worldPoint);
                    if (building != null)
                    {
                        restore.Add(building);
                    }

                    // Most átírtam tehát visszaviszi az eredetire
                    // float interpolation = building != null ? progress : 0f;

                    newHeights[z, x] = Mathf.Lerp(currentHeights[z, x], originalHeightMap[z, x], progress);
                }
            }

            terrainData.SetHeights(xStart, zStart, newHeights);
            yield return null;
        }

        RestoreTerrainForAffectedBuildings(restore);
    }

    private void RestoreTerrainForAffectedBuildings(HashSet<GameObject> restore)
    {
        foreach (GameObject go in restore)
        {
            int ID = go.GetComponent<BuildingView>().GetID();
            Vector3 currentPosition = go.transform.position;

            float terrainHeight = terrain.SampleHeight(currentPosition);

            Vector3 newPosition = new Vector3(currentPosition.x, Mathf.Lerp(terrainHeight, currentPosition.y, 0.65f), currentPosition.z);

            go.transform.position = newPosition;

            AdjustTerrainToBuilding(go, ID, false);

        }

        restore.Clear();
    }
}

