using System;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(TerrainController))]
[RequireComponent(typeof(PreviewSystem))]
public class PlacementManager : MonoBehaviour
{
    public static PlacementManager Instance;

    // Ezen alapul a teljes térkép rendszer nem ér elbaszni
    public MapData MapData;
    // Minden építési SO
    private List<BuildingData> BuildingDatabase = new List<BuildingData>();

    [SerializeField] private NavMeshSurface roadNavMesh;
    [SerializeField] private NavMeshSurface terrainNavMesh;
    [SerializeField] private Grid normalGrid;
    [SerializeField] private Grid roadGrid;

    private Grid activeGrid;
    private Vector3Int LastDetectedPosition = Vector3Int.zero;
    private IBuildingState BuildingState;

    public event Action OnPlace;
    public event Action<int> OnRemoved;

    private PreviewSystem previewSystem;
    private TerrainController terrainController;

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
        if (MapData == null) MapData = new MapData(); // Change it to load the existing placed objects
        LoadAllStructures();

        terrainController = GetComponent<TerrainController>();
        previewSystem = GetComponent<PreviewSystem>();

        LoadPreplacedStructures();
        StopPlacement();
    }

    void Update()
    {
        if (InputManager.Instance.state != State.PlacementMode) return;
        Vector3 mousePosition = InputManager.Instance.GetSelectedMapPosition();
        Vector3Int gridPosition = activeGrid.WorldToCell(mousePosition);

        if (LastDetectedPosition != gridPosition)
        {
            BuildingState.UpdateState(mousePosition);
            LastDetectedPosition = gridPosition;
        }
    }

    public void StartPlacingItem(int StructureID)
    {
        InputManager.Instance.SetState(State.PlacementMode);
        InputEventChannel.OnClick += TryPlacement;
        InputManager.Instance.StopPlacement += StopPlacement;

        StopPlacement();

        BuildingData Data = BuildingDatabase.FirstOrDefault(t => t.BuildingID == StructureID);

        if (Data.IsUnityNull()) return;
        switch (Data.type)
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

    public void StopPlacement()
    {
        if (BuildingState == null) return;
        BuildingState.EndState();
        BuildingState = null;

        InputManager.Instance.SetState(State.NormalMode);
        InputEventChannel.OnClick -= TryPlacement;
        InputManager.Instance.StopPlacement -= StopPlacement;
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

        terrainController.AdjustTerrainToStructure(newStructureGO, view.GetID(), true);

        if (Data.type == BuildingType.Road)
        {
            roadNavMesh.BuildNavMesh();
            newStructureGO.transform.SetParent(roadNavMesh.transform, true);
        }

        return view;
    }

    public void RemoveStructure(IPlaceable placeable)
    {
        OnRemoved?.Invoke(placeable.GetID());
        Vector3Int gridPosition = normalGrid.WorldToCell(placeable.GetGameObject().transform.position);

        Vector2Int GridPosition = new Vector2Int(gridPosition.x, gridPosition.z);

        MapData.RemoveObjectAt(GridPosition);
        terrainController.RestoreTerrain(placeable.GetID());

        if (placeable.GetBuildingType() == BuildingType.Road) roadNavMesh.BuildNavMesh();

        Destroy(placeable.GetGameObject());
    }

    // Megnézi van-e azon a pozicion egy GameObject ha igen visszaadja ha nem akkor nullt ad
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

    // Betölti a Resource folderból az összes StructureData ScriptableObjectet
    private void LoadAllStructures()
    {
        BuildingDatabase = new List<BuildingData>(Resources.LoadAll<BuildingData>("Buildings"));
        Debug.Log($"Betöltve {BuildingDatabase.Count} épület.");
    }

    private void LoadPreplacedStructures()
    {
        GameObject preplacedParent = GameObject.Find("PreplacedStructures");
        Debug.Log(preplacedParent.IsUnityNull());
        bool success = true;
        foreach (IPlaceable placeable in preplacedParent.GetComponentsInChildren<IPlaceable>())
        {
            BuildingData data = placeable.GetData();
            int iD = IDGenerator.GenerateID();

            Vector3 worldPosition = placeable.GetGameObject().transform.position;
            Vector3Int gridPosition = normalGrid.WorldToCell(worldPosition);
            Vector2Int mapPosition = new Vector2Int(gridPosition.x, gridPosition.z); // Csak azért, hogyha később az utakat is betöltjük

            MapData.AddObjectAt(mapPosition, data.SpaceTaken, data.BuildingID, iD);

            success = StructureManager.Instance.RegisterStructures(data, iD, placeable) && success;
        }

        string msg = success
            ? "The structures were loaded successfully."
            : "An error occurred while loading the structures.";
        GameEvents.Instance.RequestAlert(success, msg, displayTime: 5f);
    }
}

public interface IBuildingState
{
    void EndState();
    void OnAction(Vector3 gridPosition);
    void UpdateState(Vector3 gridPosition);
}