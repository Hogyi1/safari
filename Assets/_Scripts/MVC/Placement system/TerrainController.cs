using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TerrainController : MonoBehaviour
{
    [SerializeField] private Terrain terrain;

    [SerializeField]
    private float restoreSpeed = 5f;
    [SerializeField]
    private float blendingArea = 1f;


    public GameObject CurrentObject;

    TerrainData terrainData;

    // Terrain világ koordinátában
    Vector3 terrainWorldPos;

    // Structure világ koordinátái
    Vector3 StructurePos;

    // Structure sarkok világ koordinátái
    Bounds bounds;

    // Terrain szélesség hosszúság világ koordinátában 100-600-100
    Vector3 terrainSize;

    // Normalizált magasság ahol a Structure alja van
    float targetHeightInHeightMap;

    // Resolution 513
    int res;

    // xStart, yStart, Index - heightMap
    private Dictionary<Vector3Int, float[,]> storedHeights = new Dictionary<Vector3Int, float[,]>();

    private void Start()
    {
        // Terrain
        terrain = Terrain.activeTerrain;

        // Terrain és Structure adatainak beállítása
        terrainData = terrain.terrainData;

        // Terrain világ koordinátában
        terrainWorldPos = terrain.transform.position;

        // Terrain szélesség hosszúság világ koordinátában 100-600-100
        terrainSize = terrainData.size;

        // Resolution 513
        res = terrainData.heightmapResolution;
    }

    public void AdjustTerrainToStructure(GameObject Structure, int StructureIndex, bool saveOriginal)
    {
        if (terrain == null) return;

        // Beállítja a közös változókat
        SetCurrentData(Structure);

        // Elmenti az eredeti magasságokat, hogyha visszaakarnánk állítani
        SaveOriginalHeightMap(StructureIndex, saveOriginal);

        // felhúzza a domborzatot az épület aljáig
        FlattenTerrainUnderStructure();

        // Eleggyengeti a körzetében
        FlattenTerrainNearbyStructure();
    }

    private void SetCurrentData(GameObject Structure)
    {

        // A jelenlegire beállítom
        CurrentObject = Structure;

        // Structure világ koordinátái
        StructurePos = Structure.transform.position;


        // Structure tényleges alja
        try
        {
            GameObject bottom = CurrentObject.transform.Find("Floor").gameObject;
            bounds = bottom.GetComponent<Renderer>().bounds;
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex);

            // Ha nincs Floor akkor az alapot használjuk
            bounds = Structure.GetComponentInChildren<Renderer>().bounds;
            Debug.LogWarning("Floor nem található, Renderer bounds lesz használva!");
        }

        // A Structure aljának koordinátája átváltva - normalizálva heightmapre
        targetHeightInHeightMap = (bounds.min.y - terrainWorldPos.y) / terrainData.size.y;
    }

    private void FlattenTerrainUnderStructure()
    {

        // A Structure relatív koordinátája a terrainhez képest
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

    private void FlattenTerrainNearbyStructure()
    {
        // Offset az épület nagysága szerint
        Vector3 offset = new Vector3(blendingArea, 0, blendingArea);

        // Koordináta amihez nézze az eltolást
        Vector3 offsetPosition = new Vector3(bounds.min.x - offset.x, bounds.min.y, bounds.min.z - offset.z);

        // A hosszúság eltolással
        float offsetDepth = bounds.size.z + 2 * offset.z;
        float offsetWidth = bounds.size.x + 2 * offset.x;

        // A Structure relatív koordinátája a terrainhez képest
        Vector3 relativeCorner = offsetPosition - terrainWorldPos; // min x,z → bal alsó első - szemből

        // Terrain koordináta a világ koordinátából átváltva
        int xStart = Mathf.CeilToInt((relativeCorner.x / terrainSize.x) * res);
        int zStart = Mathf.FloorToInt((relativeCorner.z / terrainSize.z) * res);

        // Terrain szélesség a világ koordinátából átváltva
        int width = Mathf.CeilToInt((offsetWidth / terrainSize.x) * res); // - X
        int depth = Mathf.CeilToInt((offsetDepth / terrainSize.z) * res); // - Z

        float[,] currentHeightMap = new float[width, depth];

        try
        {
            currentHeightMap = terrainData.GetHeights(xStart, zStart, width, depth);
        }
        catch (Exception e) { Debug.LogWarning("Can't access terrain heights"); return; }

        float[,] newHeights = new float[depth, width];

        // Legtávolabbi távolság a pontok és a ház között
        float maxDistance2 = Vector3.Distance(bounds.min, relativeCorner);
        float maxDistance = Vector3.Distance(bounds.min, new Vector3(bounds.min.x - offset.x, bounds.min.y, bounds.min.z));

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
                Vector3 worldPoint = new Vector3(worldX, StructurePos.y, worldZ);

                float distance = Vector3.Distance(worldPoint, bounds.ClosestPoint(worldPoint));

                // Foglalt-e már a hely
                GameObject canDeform = PlacementManager.Instance.IsEmpty(worldPoint);


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

        // A Structure relatív koordinátája a terrainhez képest
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

        // A kulcs, xStart, zStart és a StructureIndex-ből áll
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

    public void RestoreTerrain(int StructureIndex)
    {
        Vector3Int RestoreKey = Vector3Int.zero;
        float[,] heightMap = null;

        foreach (Vector3Int key in storedHeights.Keys)
        {
            if (key.z == StructureIndex)
            {
                heightMap = storedHeights[key];
                RestoreKey = key;
                storedHeights.Remove(key);
                break;
            }
        }

        if (RestoreKey == Vector3Int.zero)
        {
            Debug.LogWarning("Ilyen ID-val rendelkező építmény nem létezik a domborzaton: " + StructureIndex);
            return;
        }

        StartCoroutine(SmoothRestore(RestoreKey.x, RestoreKey.y, heightMap));
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
                    Vector3 worldPoint = new Vector3(worldX, StructurePos.y, worldZ);

                    // Foglalt-e már a hely
                    GameObject Structure = PlacementManager.Instance.IsEmpty(worldPoint);
                    if (Structure != null)
                    {
                        restore.Add(Structure);
                    }

                    // Most átírtam tehát visszaviszi az eredetire
                    // float interpolation = Structure != null ? progress : 0f;
                    newHeights[z, x] = Mathf.Lerp(currentHeights[z, x], originalHeightMap[z, x], progress);
                }
            }

            terrainData.SetHeights(xStart, zStart, newHeights);
            yield return null;
        }

        RestoreTerrainForAffectedStructures(restore);
    }

    private void RestoreTerrainForAffectedStructures(HashSet<GameObject> restore)
    {
        // A körülötte lévő épületeket újra építjük
        foreach (GameObject go in restore)
        {
            int ID = go.GetComponent<IPlaceable>().GetID();

            bool isRoad = go.GetComponent<IPlaceable>().GetBuildingType() == BuildingType.Road;
            Vector3 currentPosition = go.transform.position;

            float terrainHeight = isRoad ? currentPosition.y : terrain.SampleHeight(currentPosition);

            // Valahol a jelenlegi és az eredeti közötti magasságra helyezem
            Vector3 newPosition = new Vector3(currentPosition.x, Mathf.Lerp(terrainHeight, currentPosition.y, 0.65f), currentPosition.z);

            go.transform.position = newPosition;

            if (isRoad) AdjustTerrainToStructure(go, ID, true);
            else AdjustTerrainToStructure(go, ID, false);
        }

        restore.Clear();
    }
}

