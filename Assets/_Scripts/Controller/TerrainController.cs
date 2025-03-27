using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class TerrainController : MonoBehaviour
{
    public Terrain terrain;

    float restoreSpeed = 5f;

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

    private Dictionary<Vector3, float[,]> storedHeights = new Dictionary<Vector3, float[,]>();

    public void AdjustTerrainToBuilding(GameObject building)
    {
        if (terrain == null) return;

        // Terrain és building adatainak beállítása
        terrainData = terrain.terrainData;

        // Terrain világ koordinátában
        terrainWorldPos = terrain.transform.position;

        // Building világ koordinátái
        buildingPos = building.transform.position;

        // Building sarkok világ koordinátái
        bounds = building.GetComponentInChildren<Renderer>().bounds;

        // Terrain szélesség hosszúság világ koordinátában 100-600-100
        terrainSize = terrainData.size;

        // Resolution 513
        res = terrainData.heightmapResolution;

        FlattenTerrainUnderBuilding(building);
        FlattenTerrainNearbyBuilding(building);
    }


    private void FlattenTerrainUnderBuilding(GameObject building)
    {

        // A building relatív koordinátája a terrainhez képest
        Vector3 relativeCorner = bounds.min - terrainWorldPos; // min x,z → bal alsó első - szemből
        Vector3 relativeMax = bounds.max - terrainWorldPos; // max x,z → jobb felső hátsó - szemből

        // Terrain koordináta a világ koordinátából átváltva
        int zStart = Mathf.FloorToInt((relativeCorner.z / terrainSize.z) * res);
        int xStart = Mathf.CeilToInt((relativeCorner.x / terrainSize.x) * res);

        // Terrain szélesség a világ koordinátából átváltva
        int depth = Mathf.CeilToInt((bounds.size.z / terrainSize.z) * res); // - Z
        int width = Mathf.CeilToInt((bounds.size.x / terrainSize.x) * res); // - X

        float[,] newHeights = new float[depth, width];

        // A building aljának koordinátája átváltva - normalizálva heightmapre
        targetHeightInHeightMap = (bounds.min.y - terrainWorldPos.y) / terrainData.size.y;


        Debug.Log(depth + " " + width);

        // Az új heightmap létrehozása
        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                Debug.Log(z + "  " + x);
                newHeights[z, x] = targetHeightInHeightMap;
            }
        }

        terrainData.SetHeights(xStart, zStart, newHeights);

    }

    private void FlattenTerrainNearbyBuilding(GameObject building)
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
        int zStart = Mathf.FloorToInt((relativeCorner.z / terrainSize.z) * res);
        int xStart = Mathf.CeilToInt((relativeCorner.x / terrainSize.x) * res);

        // Terrain szélesség a világ koordinátából átváltva
        int depth = Mathf.CeilToInt((offsetDepth / terrainSize.z) * res); // - Z
        int width = Mathf.CeilToInt((offsetWidth / terrainSize.x) * res); // - X

        float[,] currentHeightMap = terrainData.GetHeights(xStart, zStart, width, depth);
        float[,] newHeights = new float[depth, width];

        // Legtávolabbi távolság a pontok és a ház között
        float maxDistance = Vector3.Distance(bounds.min, relativeCorner) + 0.1f;
        Debug.Log(maxDistance + " Maxdistance ");
        Debug.Log("The offset is: " + relativeCorner);
        Debug.Log("The width and height offsets: " + offsetWidth + " " + offsetDepth);


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


        terrainData.SetHeights(xStart, zStart, newHeights);

    }

    private void SaveOriginalHeightMap(GameObject gameObject)
    {
        // Offset az épület nagysága szerint
        Vector3 offset = bounds.center - bounds.min;

        // Koordináta amihez nézze az eltolást
        Vector3 offsetPosition = new Vector3(bounds.min.x - offset.x, bounds.min.y, bounds.min.z - offset.z);

        // A hosszúság eltolással
        float offsetDepth = bounds.size.z + 2 * offset.z;
        float offsetWidth = bounds.size.x + 2 * offset.x;

        // A building relatív koordinátája a terrainhez képest
        Vector3 relativeCorner = bounds.min - terrainWorldPos; // min x,z → bal alsó első - szemből
        Vector3 relativeMax = bounds.max - terrainWorldPos; // max x,z → jobb felső hátsó - szemből

        // Terrain koordináta a világ koordinátából átváltva
        int zStart = Mathf.FloorToInt((relativeCorner.z / terrainSize.z) * res);
        int xStart = Mathf.CeilToInt((relativeCorner.x / terrainSize.x) * res);

        // Terrain szélesség a világ koordinátából átváltva
        int depth = Mathf.CeilToInt((offsetDepth / terrainSize.z) * res); // - Z
        int width = Mathf.CeilToInt((offsetWidth / terrainSize.x) * res); // - X

        float[,] originalHeightMap = terrainData.GetHeights(xStart, zStart, depth, width);
        storedHeights[gameObject.transform.position] = originalHeightMap;
    }

    public void RestoreTerrain(GameObject building)
    {
        if (!storedHeights.ContainsKey(building.transform.position))
        {
            Debug.LogWarning("No stored terrain data found for this position.");
            return;
        }

        StartCoroutine(SmoothRestore(building.transform.position));
    }

    private IEnumerator SmoothRestore(Vector3 position)
    {
        if (terrain == null) yield return null;

        TerrainData terrainData = terrain.terrainData;

        int heightmapWidth = terrainData.heightmapResolution;
        int heightmapHeight = terrainData.heightmapResolution;

        Vector3 terrainPosition = position - terrain.transform.position;
        int xCenter = Mathf.RoundToInt((terrainPosition.x / terrainData.size.x) * heightmapWidth);
        int zCenter = Mathf.RoundToInt((terrainPosition.z / terrainData.size.z) * heightmapHeight);

        float[,] originalHeights = storedHeights[position];
        int radius = originalHeights.GetLength(0);
        float[,] currentHeights = terrainData.GetHeights(xCenter - radius / 2, zCenter - radius / 2, radius, radius);

        float progress = 0f;

        while (progress < 1f)
        {
            progress += Time.deltaTime * restoreSpeed;

            float[,] newHeights = new float[radius, radius];

            for (int i = 0; i < radius; i++)
            {
                for (int j = 0; j < radius; j++)
                {
                    newHeights[i, j] = Mathf.Lerp(currentHeights[i, j], originalHeights[i, j], progress);
                }
            }

            terrainData.SetHeights(xCenter - radius / 2, zCenter - radius / 2, newHeights);
            yield return null;
        }

        terrainData.SetHeights(xCenter - radius / 2, zCenter - radius / 2, originalHeights);
        storedHeights.Remove(position);
    }

}

