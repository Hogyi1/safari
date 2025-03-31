using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlacementManager : MonoBehaviour
{
    public static PlacementManager Instance;

    [SerializeField]
    private List<BuildingData> buildingDatabase = new List<BuildingData>();

    [SerializeField]
    public MapData MapData;

    [SerializeField]
    private Grid Grid;
    private readonly int RoadCellSize = 6;
    private readonly int StructureCellSize = 1;


    private bool isPlacementModeActive = false;
    private Vector3Int LastDetectedPosition = Vector3Int.zero;

    [SerializeField]
    IBuildingState BuildingState;

    [SerializeField]
    private PreviewSystem PreviewSystem;

    [SerializeField]
    public TerrainController TerrainController;

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

    }

    void Start()
    {
        LoadAllBuildings();
        StopPlacement();
        this.MapData = new MapData();
    }

    void Update()
    {
        if (!isPlacementModeActive) return;
        Vector3 mousePosition = InputManager.Instance.GetSelectedMapPosition();
        Vector3Int gridPosition = Grid.WorldToCell(mousePosition);


        if (LastDetectedPosition != gridPosition)
        {
            BuildingState.UpdateState(mousePosition);
            LastDetectedPosition = gridPosition;
        }
    }

    public void StartPlacingItem(int BuildingID)
    {
        BuildingData Structure = buildingDatabase.Find(t => t.BuildingID == BuildingID);
        StopPlacement();
        isPlacementModeActive = Structure != null;
        int cellSize = Structure.type == BuildingType.ROAD ? RoadCellSize : StructureCellSize;
        Grid.cellSize = new Vector3(cellSize, 1, cellSize);
        PreviewSystem.SetGridSize(1f / cellSize);
        if (isPlacementModeActive)
        {
            BuildingState = new PlacementState(Structure, Grid, PreviewSystem, MapData);
            InputManager.Instance.OnClicked += TryPlacement;
            InputManager.Instance.OnExit += StopPlacement;
        }
    }

    public void StopPlacement()
    {
        if (!isPlacementModeActive) return;
        isPlacementModeActive = false;
        Grid.cellSize = new Vector3(StructureCellSize, 1, StructureCellSize);
        BuildingState.EndState();
        InputManager.Instance.OnClicked -= TryPlacement;
        InputManager.Instance.OnExit -= StopPlacement;
        LastDetectedPosition = Vector3Int.zero;
        BuildingState = null;
    }

    public void TryPlacement()
    {
        if (InputManager.Instance.IsPointerOverUI()) return;

        Vector3 mousePosition = InputManager.Instance.GetSelectedMapPosition();

        BuildingState.OnAction(mousePosition);
    }

    public BuildingView PlaceStructure(BuildingData Data, Vector3 position, Building newBuilding)
    {

        GameObject newBuildingGO = Instantiate(Data.BuildingPrefab, position, Quaternion.identity);
        BuildingView view = newBuildingGO.GetComponent<BuildingView>();
        view.Init(newBuilding);

        TerrainController.AdjustTerrainToBuilding(newBuildingGO, view.GetID(), true);

        return view;
    }

    public void RemoveStructure(BuildingView view)
    {
        Vector3Int gridPosition = Grid.WorldToCell(view.transform.position);

        int cellSize = Mathf.RoundToInt(Grid.cellSize.x);

        Vector2Int GridPosition = new Vector2Int(gridPosition.x * cellSize, gridPosition.z * cellSize);

        Debug.Log(view.transform.position + "  " + cellSize);

        MapData.RemoveObjectAt(GridPosition);
        TerrainController.RestoreTerrain(view.GetID());
        Destroy(view.gameObject);
    }

    // Megnézi van azon a pozicion egy GameObject ha igen visszaadja ha nem akkor nullt ad
    public GameObject IsEmpty(Vector3 position)
    {
        Vector3Int gridPosition = Grid.WorldToCell(position);

        int cellSize = Mathf.RoundToInt(Grid.cellSize.x);
        Vector2Int GridPosition = new Vector2Int(gridPosition.x * cellSize, gridPosition.z * cellSize);

        int buildingID = MapData.IsOccupied(GridPosition);
        if (buildingID > -1)
            return BuildingManager.Instance.buildingViews[buildingID].gameObject;
        return null;
    }

    // Betölti a Resource folderból az összes BuildingData ScriptableObjectet
    private void LoadAllBuildings()
    {
        buildingDatabase = new List<BuildingData>(Resources.LoadAll<BuildingData>("Buildings"));
        Debug.Log($"Betöltve {buildingDatabase.Count} épület.");
    }






    // A forgatáshoz szükséges komponensek
    public enum Direction
    {
        UP, DOWN, LEFT, RIGHT
    }
}
