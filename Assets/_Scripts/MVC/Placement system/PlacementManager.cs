using System;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(TerrainController))]
[RequireComponent(typeof(PreviewSystem))]
public class PlacementManager : MonoBehaviour, IPlaceableManager
{
    public static PlacementManager Instance;

    // Ezen alapul a teljes térkép rendszer nem ér elbaszni
    public MapData MapData;
    // Minden építési SO
    private List<BuildingData> buildingDatabase = new List<BuildingData>();

    [SerializeField] private NavMeshSurface roadNavMesh;
    [SerializeField] private NavMeshSurface terrainNavMesh;
    [SerializeField] private Grid normalGrid;
    [SerializeField] private Grid roadGrid;
    [SerializeField] private GameObject preplacedStructures;
    [SerializeField] private GameObject preplacedObjects;
    [SerializeField] private GameObject structureParent;

    private Grid activeGrid;
    private Vector3Int LastDetectedPosition = Vector3Int.zero;
    private IBuildingState BuildingState;

    public event Action OnPlaced;
    public event Action OnStopped;

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
        LoadPreplacedObjects();
        StopPlacement();
    }

    void Update()
    {
        if (InputManager.Instance.State != InputState.PlacementMode) return;
        Vector3 mousePosition = InputManager.Instance.GetSelectedMapPosition();
        Vector3Int gridPosition = activeGrid.WorldToCell(mousePosition);

        if (LastDetectedPosition != gridPosition)
        {
            BuildingState.UpdateState(mousePosition);
            LastDetectedPosition = gridPosition;
        }
    }

    public Vector2Int GetRoadCellByPosition(Vector3 pos)
    {
        Vector3Int cell3D = roadGrid.WorldToCell(pos);
        return new Vector2Int(cell3D.x, cell3D.z);
    }

    public void StartPlacing(int StructureID)
    {
        InputManager.Instance.SetState(InputState.PlacementMode);
        InputEventChannel.OnClick += TryPlacement;
        InputManager.Instance.StopPlacement += StopPlacement;

        StopPlacement();

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

    public void TryPlacement()
    {
        if (InputManager.Instance.IsPointerOverUI()) return;

        Vector3 mousePosition = InputManager.Instance.GetSelectedMapPosition();

        bool placed = BuildingState.OnAction(mousePosition);
        if (placed) OnPlaced?.Invoke();
    }

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

    private void LoadPreplacedStructures()
    {
        bool success = true;
        foreach (IPlaceable placeable in preplacedStructures.GetComponentsInChildren<IPlaceable>())
        {
            BuildingData data = placeable.GetData();
            int iD = IDGenerator.GenerateID();

            Vector3 worldPosition = placeable.GetGameObject().transform.position;
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
            catch (Exception e)
            {
                success = false;
            }
        }

        string msg = success
            ? "The objects were loaded successfully."
            : "An error occurred while loading the objects.";
        GameEvents.Instance.RequestAlert(success, msg, displayTime: 5f);
    }

    // Betölti a Resource folderból az összes StructureData ScriptableObjectet
    private void LoadAllStructures()
    {
        buildingDatabase = new List<BuildingData>(Resources.LoadAll<BuildingData>("Buildings"));
        Debug.Log($"Betöltve {buildingDatabase.Count} épület.");
    }
}

public interface IBuildingState
{
    void EndState();
    bool OnAction(Vector3 gridPosition);
    void UpdateState(Vector3 gridPosition);
}