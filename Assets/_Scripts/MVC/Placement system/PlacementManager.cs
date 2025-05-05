using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    public static PlacementManager Instance;

    // Ezen alapul a teljes térkép rendszer nem ér elbaszni
    public MapData MapData;
    // Minden építési SO
    private List<BuildingData> BuildingDatabase = new List<BuildingData>();

    [SerializeField] private NavMeshSurface RoadNavMesh;
    [SerializeField] private Grid NormalGrid;
    [SerializeField] private Grid RoadGrid;
    [SerializeField] private Grid ActiveGrid;
    private Vector3Int LastDetectedPosition = Vector3Int.zero;

    public event Action OnPlace;

    [SerializeField] IBuildingState BuildingState;

    [SerializeField] private PreviewSystem PreviewSystem;

    [SerializeField] public TerrainController TerrainController;

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
        if (MapData == null) MapData = new MapData();
        LoadAllStructures();
        // StopPlacement();
    }

    void Update()
    {
        if (InputManager.Instance.state != State.PlacementMode) return;
        Vector3 mousePosition = InputManager.Instance.GetSelectedMapPosition();
        Vector3Int gridPosition = ActiveGrid.WorldToCell(mousePosition);

        if (LastDetectedPosition != gridPosition)
        {
            BuildingState.UpdateState(mousePosition);
            LastDetectedPosition = gridPosition;
        }
    }

    public void StartPlacingItem(int StructureID)
    {
        InputManager.Instance.SetState(State.PlacementMode);
        InputManager.Instance.OnClicked += TryPlacement;
        InputManager.Instance.OnExit += StopPlacement;

        StopPlacement();

        BuildingData Data = BuildingDatabase.FirstOrDefault(t => t.BuildingID == StructureID);

        if (Data.IsUnityNull()) return;
        switch (Data.type)
        {
            case BuildingType.Road:
                ActiveGrid = RoadGrid;
                BuildingState = new RoadState(Data, ActiveGrid, PreviewSystem, MapData);
                PreviewSystem.SetGridSize(6f);
                break;
            default:
                ActiveGrid = NormalGrid;
                BuildingState = new PlacementState(Data, ActiveGrid, PreviewSystem, MapData);
                PreviewSystem.SetGridSize(1f);
                break;
        }
    }

    public void StopPlacement()
    {
        if (BuildingState == null) return;
        BuildingState.EndState();
        BuildingState = null;

        InputManager.Instance.SetState(State.NormalMode);
        InputManager.Instance.OnClicked -= TryPlacement;
        InputManager.Instance.OnExit -= StopPlacement;
    }

    public void TryPlacement()
    {
        if (InputManager.Instance.IsPointerOverUI()) return;

        Vector3 mousePosition = InputManager.Instance.GetSelectedMapPosition();

        BuildingState.OnAction(mousePosition);
    }

    public IPlaceable PlaceStructure(BuildingData Data, Vector3 position, Structure newStructure)
    {
        OnPlace?.Invoke();
        GameObject newStructureGO = Instantiate(Data.BuildingPrefab, position, Quaternion.identity);
        IPlaceable view = newStructureGO.GetComponent<IPlaceable>();
        view.Init(newStructure);

        TerrainController.AdjustTerrainToStructure(newStructureGO, view.GetID(), true);

        return view;
    }

    public IPlaceable PlaceRoad(BuildingData Data, Vector3 position)
    {
        OnPlace?.Invoke();
        GameObject newStructureGO = Instantiate(Data.BuildingPrefab, position, Quaternion.identity);
        IPlaceable view = newStructureGO.GetComponent<IPlaceable>();
        newStructureGO.transform.SetParent(RoadNavMesh.transform, true);

        TerrainController.AdjustTerrainToStructure(newStructureGO, view.GetID(), true);

        RoadNavMesh.BuildNavMesh();
        return view;
    }

    public void RemoveStructure(IPlaceable placeable)
    {
        Vector3Int gridPosition = ActiveGrid.WorldToCell(placeable.GetGameObject().transform.position);

        int cellSize = Mathf.RoundToInt(ActiveGrid.cellSize.x);

        Vector2Int GridPosition = new Vector2Int(gridPosition.x * cellSize, gridPosition.z * cellSize);

        MapData.RemoveObjectAt(GridPosition);
        TerrainController.RestoreTerrain(placeable.GetID());
        if (placeable is RoadView road)
        {
            RoadNavMesh.BuildNavMesh();
        }
        Destroy(placeable.GetGameObject());
    }

    // Megnézi van-e azon a pozicion egy GameObject ha igen visszaadja ha nem akkor nullt ad
    public GameObject IsEmpty(Vector3 position)
    {
        Vector3Int gridPosition = NormalGrid.WorldToCell(position);

        Vector2Int GridPosition = new Vector2Int(gridPosition.x, gridPosition.z);

        int StructureID = MapData.IsOccupied(GridPosition);
        if (StructureID > -1)
        {
            if (StructureManager.Instance.IInteractables.TryGetValue(StructureID, out IPlaceable placeable)) return placeable.GetGameObject();
            else { RoadManager.Instance.ActiveViews.TryGetValue(StructureID, out RoadView roadview); return roadview.GetGameObject(); }
        }
        return null;
    }

    // Betölti a Resource folderból az összes StructureData ScriptableObjectet
    private void LoadAllStructures()
    {
        BuildingDatabase = new List<BuildingData>(Resources.LoadAll<BuildingData>("Buildings"));
        Debug.Log($"Betöltve {BuildingDatabase.Count} épület.");
    }

    // Minden előre telepített fa helyét lefoglalja
    public void SetTreePositions(List<Vector3> treePositions)
    {
        foreach (var pos in treePositions)
        {
            Vector3Int gridpos = NormalGrid.WorldToCell(pos);
            Vector2Int gridPosition = new Vector2Int(gridpos.x, gridpos.z);
            Vector2Int objectSize = new Vector2Int(2, 2);
            if (MapData == null) MapData = new MapData();
            MapData.AddObjectAt(gridPosition, objectSize, -1, -1);
        }
    }
}

public interface IBuildingState
{
    void EndState();
    void OnAction(Vector3 gridPosition);
    void UpdateState(Vector3 gridPosition);
}