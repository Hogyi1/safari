using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Manages the placement of structures on the map, including preview handling,
/// terrain adjustment, grid calculation, and integration with the save/load system.
/// Singleton-based manager that connects with building data and placement state.
/// </summary>
[RequireComponent(typeof(TerrainController))]
[RequireComponent(typeof(PreviewSystem))]
public class PlacementManager : MonoBehaviour, IPlaceableManager, IDataPersistence
{
    /// <summary>
    /// Singleton instance of the PlacementManager.
    /// </summary>
    public static PlacementManager Instance;

    /// <summary>
    /// Stores all spatial information about placed structures and occupancy.
    /// </summary>
    public MapData MapData;

    /// <summary>
    /// Internal list containing all building definitions loaded from Resources.
    /// </summary>
    private List<BuildingData> buildingDatabase = new List<BuildingData>();

    /// <summary>
    /// NavMesh surface used for updating navigation on roads.
    /// </summary>
    [SerializeField] private NavMeshSurface roadNavMesh;

    /// <summary>
    /// NavMesh surface used for terrain areas (non-road).
    /// </summary>
    [SerializeField] private NavMeshSurface terrainNavMesh;

    /// <summary>
    /// Grid used for general structure placement (1x1).
    /// </summary>
    [SerializeField] private Grid normalGrid;

    /// <summary>
    /// Grid used for road structure placement (6x6).
    /// </summary>
    [SerializeField] private Grid roadGrid;

    /// <summary>
    /// Parent GameObject containing all pre-placed structures in the scene.
    /// </summary>
    [SerializeField] private GameObject preplacedStructures;

    /// <summary>
    /// Parent GameObject containing decorative or environmental objects.
    /// </summary>
    [SerializeField] private GameObject preplacedObjects;

    /// <summary>
    /// Parent transform under which dynamically placed structures are organized.
    /// </summary>
    [SerializeField] private GameObject structureParent;

    /// <summary>
    /// Stores positions of all pre-placed structures to prevent duplication.
    /// </summary>
    private HashSet<Vector3> preplacedPositions = new();

    /// <summary>
    /// Currently active grid based on selected structure type (road or normal).
    /// </summary>
    private Grid activeGrid;

    /// <summary>
    /// Last detected grid position under the mouse cursor.
    /// Used to prevent unnecessary update calls.
    /// </summary>
    private Vector3Int LastDetectedPosition = Vector3Int.zero;

    /// <summary>
    /// Current placement state handling logic for structure preview and action.
    /// </summary>
    private IBuildingState BuildingState;

    /// <summary>
    /// Called when a structure is successfully placed.
    /// </summary>
    public event Action OnPlaced;

    /// <summary>
    /// Called when the placement mode is exited or cancelled.
    /// </summary>
    public event Action OnStopped;

    /// <summary>
    /// Handles the live placement preview logic.
    /// </summary>
    private PreviewSystem previewSystem;

    /// <summary>
    /// Controls terrain deformation for placed structures.
    /// </summary>
    private TerrainController terrainController;

    /// <summary>
    /// Priority value for determining loading order during save/load. Lower values load first.
    /// </summary>
    public float Priority => 3000f;


    /// <summary>
    /// Initializes the singleton instance, loads structure data, and prepares systems.
    /// </summary>
    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadAllStructures();
        MapData = new MapData();

        terrainController = GetComponent<TerrainController>();
        previewSystem = GetComponent<PreviewSystem>();


        StopPlacement();
    }

    /// <summary>
    /// Loads decorative (non-functional) objects into the map reservation system.
    /// </summary>
    private void Start()
    {
        // LoadPreplacedStructures(); //KICSERÉLNI CREATEMAPRA
        LoadPreplacedObjects();
    }

    //// <summary>
    /// Loads decorative (non-functional) objects into the map reservation system.
    /// </summary>
    void Update()
    {
        if (InputManager.Instance.State != InputState.PlacementMode && BuildingState.IsUnityNull()) return;
        Vector3 mousePosition = InputManager.Instance.GetSelectedMapPosition();
        Vector3Int gridPosition = activeGrid.WorldToCell(mousePosition);

        if (LastDetectedPosition != gridPosition)
        {
            BuildingState.UpdateState(mousePosition);
            LastDetectedPosition = gridPosition;
        }
    }


    /// <summary>
    /// Converts a world position into 2D road grid coordinates.
    /// </summary>
    /// <param name="pos">The world position to convert.</param>
    /// <returns>Grid position in road coordinates.</returns>
    public Vector2Int GetRoadCellByPosition(Vector3 pos)
    {
        Vector3Int cell3D = roadGrid.WorldToCell(pos);
        return new Vector2Int(cell3D.x, cell3D.z);
    }


    /// <summary>
    /// Begins the placement process for a structure with the given ID.
    /// Sets active grid and preview mode.
    /// </summary>
    /// <param name="StructureID">The ID of the structure to place.</param>
    public void StartPlacing(int StructureID)
    {
        StopPlacement();

        InputManager.Instance.SetState(InputState.PlacementMode);
        InputEventChannel.OnClick += TryPlacement;
        InputManager.Instance.StopPlacement += StopPlacement;

        BuildingData Data = buildingDatabase.FirstOrDefault(t => t.BuildingID == StructureID);

        if (Data.IsUnityNull()) return;
        switch (Data.Type)
        {
            case BuildingType.Road:
                activeGrid = roadGrid;
                previewSystem.SetGridSize(6f);
                break;
            default:
                activeGrid = normalGrid;
                previewSystem.SetGridSize(1f);
                break;
        }

        BuildingState = new PlacementState(Data, activeGrid, previewSystem, MapData);
    }


    /// <summary>
    /// Cancels any active structure placement and clears input bindings.
    /// </summary>
    public void StopPlacement()
    {
        if (BuildingState == null) return;
        BuildingState.EndState();
        BuildingState = null;

        OnStopped?.Invoke();

        InputManager.Instance.SetState(InputState.NormalMode);
        InputEventChannel.OnClick -= TryPlacement;
        InputManager.Instance.StopPlacement -= StopPlacement;
    }


    /// <summary>
    /// Attempts to place the currently selected structure at the current mouse position.
    /// </summary>
    public void TryPlacement()
    {
        if (InputManager.Instance.IsPointerOverUI()) return;

        Vector3 mousePosition = InputManager.Instance.GetSelectedMapPosition();

        bool placed = BuildingState.OnAction(mousePosition);
        if (placed) OnPlaced?.Invoke();
    }


    /// <summary>
    /// Instantiates and places a structure in the scene and registers it with systems.
    /// </summary>
    /// <param name="Data">The building data used for placement.</param>
    /// <param name="position">World position to place the structure.</param>
    /// <param name="newStructure">The structure logic object to bind to the view.</param>
    /// <returns>The initialized IPlaceable component of the structure.</returns>
    public IPlaceable Place(BuildingData Data, Vector3 position, Structure newStructure)
    {
        GameObject newStructureGO = Instantiate(Data.BuildingPrefab, position, Quaternion.identity);
        newStructureGO.transform.SetParent(structureParent.transform, true);
        IPlaceable view = newStructureGO.GetComponent<IPlaceable>();
        view.Init(newStructure);

        terrainController.AdjustTerrainToStructure(newStructureGO, view.GetID(), true);

        if (Data.Type == BuildingType.Road)
        {
            roadNavMesh.BuildNavMesh();
            newStructureGO.transform.SetParent(roadNavMesh.transform, true);
        }
        return view;
    }


    /// <summary>
    /// Removes a structure from the scene, map, terrain, and inventory systems.
    /// </summary>
    /// <param name="placeable">The structure view to remove.</param>
    public void RemoveStructure(IPlaceable placeable)
    {
        Vector3Int gridPosition = normalGrid.WorldToCell(placeable.GetGameObject().transform.position);
        Vector2Int GridPosition = new Vector2Int(gridPosition.x, gridPosition.z);

        InventoryManager.Instance.PickedUpItem(placeable.GetData().BuildingID);
        MapData.RemoveObjectAt(GridPosition);
        terrainController.RestoreTerrain(placeable.GetID());

        if (placeable.GetBuildingType() == BuildingType.Road) roadNavMesh.BuildNavMesh();

        Destroy(placeable.GetGameObject());
    }



    /// <summary>
    /// Checks if a given world position is already occupied by a structure.
    /// </summary>
    /// <param name="position">The position to check.</param>
    /// <returns>The GameObject at the position, or null if unoccupied.</returns>
    public GameObject IsEmpty(Vector3 position)
    {
        Vector3Int gridPosition = normalGrid.WorldToCell(position);

        Vector2Int GridPosition = new Vector2Int(gridPosition.x, gridPosition.z);

        int StructureID = MapData.IsOccupied(GridPosition);
        if (StructureID > -1)
        {
            IPlaceable placeable = StructureManager.Instance.GetCorrespondingView(StructureID);
            return placeable.GetGameObject();
        }
        return null;
    }


#warning KICSERÉLNI A CREATE MAPRA
    /// <summary>
    /// Loads all pre-placed structures from the scene into the map and structure registry.
    /// Handles road types specially due to alignment needs.
    /// </summary>
    private void LoadPreplacedStructures()
    {
        bool success = true;
        foreach (IPlaceable placeable in preplacedStructures.GetComponentsInChildren<IPlaceable>())
        {
            BuildingData data = placeable.GetData();
            int iD = IDGenerator.GenerateID();

            Vector3 worldPosition = placeable.GetGameObject().transform.position;
            preplacedPositions.Add(worldPosition);
            Vector3Int gridPosition = normalGrid.WorldToCell(worldPosition);
            Vector2Int mapPosition = new Vector2Int(gridPosition.x, gridPosition.z);
            Vector3Int roadPosition = Vector3Int.zero;

            // EZ kibaszott felesleges ha már van rajta épület
            if (data.Type == BuildingType.Road)
            {
                Vector3 middle = placeable.GetGameObject().transform.Find("Middle").position; // közepe hogy benn legyen pont a 6x6ban
                roadPosition = roadGrid.WorldToCell(middle); // 6x6 helye
                Vector3 normalPosition = roadGrid.CellToWorld(roadPosition); // sarka az 1x1-nek world pos
                Vector3Int newGridPosition = normalGrid.WorldToCell(normalPosition); // cell pos az 1x1-ben
                mapPosition = new Vector2Int(newGridPosition.x, newGridPosition.z);
            }
            Vector2Int nodePosition = new Vector2Int(roadPosition.x, roadPosition.z); // Csak azért, hogyha később az utakat is betöltjük

            MapData.AddObjectAt(mapPosition, data.SpaceTaken, data.BuildingID, iD);

            success = StructureManager.Instance.RegisterStructures(data, iD, placeable, nodePosition) && success;
        }

        string msg = success
            ? "The structures were loaded successfully."
            : "An error occurred while loading the structures.";
        GameEvents.Instance.RequestAlert(success, msg, displayTime: 5f);
    }


    // <summary>
    /// Loads passive map objects that does not have a model layer into the map occupancy system.
    /// </summary>
    private void LoadPreplacedObjects()
    {
        bool success = true;
        foreach (Transform child in preplacedObjects.transform)
        {
            GameObject go = child.gameObject;
            Bounds bounds = go.GetComponent<Renderer>().bounds;

            Vector3 min = bounds.min;
            Vector3 max = bounds.max;

            Vector3Int leftDownCorner = normalGrid.WorldToCell(min);
            Vector3Int rightUpperCorner = normalGrid.WorldToCell(max);

            Vector3Int difference = rightUpperCorner - leftDownCorner;
            Vector2Int spaceTaken = new Vector2Int(difference.x, difference.z);
            Vector2Int mapPosition = new Vector2Int(leftDownCorner.x, leftDownCorner.z);

            try
            {
                MapData.AddObjectAt(mapPosition, spaceTaken, -999, -999);
            }
            catch (Exception)
            {
                success = false;
            }
        }

        string msg = success
            ? "The objects were loaded successfully."
            : "An error occurred while loading the objects.";
        GameEvents.Instance.RequestAlert(success, msg, displayTime: 5f);
    }


    /// <summary>
    /// Loads all available BuildingData assets from Resources/Buildings.
    /// </summary>
    private void LoadAllStructures()
    {
        buildingDatabase = new List<BuildingData>(Resources.LoadAll<BuildingData>("Buildings"));
        Debug.Log($"Betöltve {buildingDatabase.Count} épület.");
    }


    /// <summary>
    /// Coroutine that places all previously saved structures after loading game data.
    /// </summary>
    /// <param name="data">The GameData containing saved structures.</param>
    /// <returns>Enumerator for coroutine execution.</returns>
    public IEnumerator LoadData(GameData data)
    {
        yield return PlaceLate(data);
    }


    /// <summary>
    /// Saves all placed structure data and positions to the provided GameData object.
    /// </summary>
    /// <param name="data">The GameData to write into.</param>
    public void SaveData(GameData data)
    {
        List<StructureSaveData> structureSaves = new();
        List<MapSaveData> mapSaves = new();

        StructureManager.Instance.GetStructures().ForEach(t => structureSaves.Add(t.GetSaveData()));
        StructureManager.Instance.GetPlaceables().ForEach(t =>
        {
            var uID = t.GetID();
            var bID = t.GetData().BuildingID;
            var go = t.GetGameObject();
            mapSaves.Add(new MapSaveData(uID, bID, go.transform.position, go.transform.rotation));
        });

        data.mapDatas = mapSaves;
        data.structureDatas = structureSaves;
    }


    /// <summary>
    /// Saves all placed structure data and positions to the provided GameData object.
    /// </summary>
    /// <param name="data">The GameData to write into.</param>
    private BuildingData GetBuildingDataByBuildingID(int buildingID)
    {
        return buildingDatabase.FirstOrDefault(t => t.BuildingID == buildingID);
    }


    /// <summary>
    /// Coroutine that restores all structures from saved data after one frame delay.
    /// Also updates the road navmesh and registers nodes if applicable.
    /// </summary>
    /// <param name="data">The GameData containing all map and structure info.</param>
    /// <returns>Coroutine enumerator.</returns>
    private IEnumerator PlaceLate(GameData data)
    {
        yield return new WaitForEndOfFrame();

        List<StructureSaveData> structureSaves = data.structureDatas;
        List<MapSaveData> mapSaves = data.mapDatas;

        for (int i = 0; i < mapSaves.Count; ++i)
        {
            var map = mapSaves[i];
            var structure = structureSaves[i];

            BuildingData Buildingdata = GetBuildingDataByBuildingID(map.BuildingID);

            Vector3Int gridPosition = normalGrid.WorldToCell(map.Position);
            Vector2Int mapPosition = new Vector2Int(gridPosition.x, gridPosition.z);
            Vector3Int roadPosition = Vector3Int.zero;
            Vector2Int nodePosition = new Vector2Int(roadPosition.x, roadPosition.z);

            GameObject newStructureGO = Instantiate(Buildingdata.BuildingPrefab, map.Position, map.Rotation);
            newStructureGO.transform.SetParent(structureParent.transform, true);
            IPlaceable view = newStructureGO.GetComponent<IPlaceable>();

            if (Buildingdata.Type == BuildingType.Road)
            {
                Vector3 middle = view.GetGameObject().transform.Find("Middle").position;
                roadPosition = roadGrid.WorldToCell(middle);
                Vector3 normalPosition = roadGrid.CellToWorld(roadPosition);
                Vector3Int newGridPosition = normalGrid.WorldToCell(normalPosition);
                mapPosition = new Vector2Int(newGridPosition.x, newGridPosition.z);
                nodePosition = new Vector2Int(roadPosition.x, roadPosition.z);

                roadNavMesh.BuildNavMesh();
                newStructureGO.transform.SetParent(roadNavMesh.transform, true);
            }

            MapData.AddObjectAt(mapPosition, Buildingdata.SpaceTaken, Buildingdata.BuildingID, structure.UniqueID);
            StructureManager.Instance.RegisterStructures(Buildingdata, structure, view, nodePosition);
        }
    }
}


/// <summary>
/// Defines the behavior contract for placing structures in the game.
/// Used by the placement system to manage structure preview, validation,
/// and final placement based on player input.
/// </summary>
public interface IBuildingState
{
    /// <summary>
    /// Cleans up any active state, such as preview objects or input bindings.
    /// Called when the placement mode is cancelled or completed.
    /// </summary>
    void EndState();

    /// <summary>
    /// Attempts to place the structure at the given grid position.
    /// </summary>
    /// <param name="gridPosition">The world position where placement is attempted.</param>
    /// <returns>True if the placement was successful; otherwise, false.</returns>
    bool OnAction(Vector3 gridPosition);

    /// <summary>
    /// Updates the preview or internal logic based on the current mouse/grid position.
    /// Called continuously during placement mode.
    /// </summary>
    /// <param name="gridPosition">The world position currently targeted by the player.</param>
    void UpdateState(Vector3 gridPosition);
}
