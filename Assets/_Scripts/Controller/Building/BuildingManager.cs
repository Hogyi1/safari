using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildingManager : MonoBehaviour, IRandomEventObserver
{
    public static BuildingManager Instance;

    [SerializeField]
    private List<BuildingData> buildingDatabase = new List<BuildingData>();

    [SerializeField]
    private int nextID = 0;

    [SerializeField]
    private List<Building> activeBuildings = new List<Building>();

    [SerializeField]
    private Dictionary<int, BuildingView> buildingViews = new Dictionary<int, BuildingView>();

    [SerializeField]
    private Building ActiveBuilding = null;
    [SerializeField]
    private BuildingView ActiveView = null;
    [SerializeField]
    private BuildingView LastView = null;

    [SerializeField]
    public MapData MapData;

    [SerializeField]
    private Grid grid;

    [SerializeField]
    private Material ActiveMaterial;

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
        // RandomEvents.Instance.AddObserver(this);
        LoadAllBuildings();

        StopPlacement();
        this.MapData = new MapData();

        InputManager.Instance.OnClicked += SetViewActive;
        InputManager.Instance.OnClicked += SetViewInActive;
    }

    // Update is called once per frame
    void Update()
    {
        BuildingView hoveredObject = InputManager.Instance.GetHoveredObjectScript();
        if (!isPlacementModeActive)
        {

            if (hoveredObject != LastView)
            {
                if (LastView != null)
                {
                    LastView.isHovered = false;
                }

                LastView = hoveredObject;

                if (LastView != null)
                {
                    LastView.isHovered = true;
                }

            }

            if (hoveredObject == null && LastView != null)
            {
                LastView.isHovered = false;
                //LastView = null;
            }
            return;
        }

        Vector3 mousePosition = InputManager.Instance.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);


        if (LastDetectedPosition != gridPosition)
        {
            BuildingState.UpdateState(mousePosition);
            LastDetectedPosition = gridPosition;
        }
    }

    private void SetViewInActive()
    {
        if (ActiveView != null)
        {
            ActiveView.isActive = false;
            ActiveView.HideUI();
            ActiveView = null;
        }
    }

    private void SetViewActive()
    {
        if (LastView != null)
        {
            SetViewInActive();
            ActiveView = LastView;
            ActiveView.isActive = true;
            ActiveView.ShowUI(ActiveMaterial);
        }
    }

    public void StartPlacingItem(int BuildingID)
    {
        BuildingData Structure = buildingDatabase.Find(t => t.BuildingID == BuildingID);
        StopPlacement();
        isPlacementModeActive = Structure != null;
        if (isPlacementModeActive)
        {
            BuildingState = new PlacementState(Structure, grid, PreviewSystem, MapData);
            InputManager.Instance.OnClicked -= SetViewActive;
            InputManager.Instance.OnClicked += PlaceStructure;
            InputManager.Instance.OnExit += StopPlacement;
        }

    }

    public void StopPlacement()
    {
        if (!isPlacementModeActive) return;
        isPlacementModeActive = false;
        BuildingState.EndState();
        InputManager.Instance.OnClicked += SetViewActive;
        InputManager.Instance.OnClicked -= PlaceStructure;
        InputManager.Instance.OnExit -= StopPlacement;
        LastDetectedPosition = Vector3Int.zero;
        BuildingState = null;
    }

    private void PlaceStructure()
    {
        if (InputManager.Instance.IsPointerOverUI()) return;

        Vector3 mousePosition = InputManager.Instance.GetSelectedMapPosition();

        BuildingState.OnAction(mousePosition);
    }

    public int AddBuilding(BuildingData Data, Vector3 position)
    {
        int GeneratedID = GenerateID();

        Building newBuilding = new Building(GeneratedID, Data);
        activeBuildings.Add(newBuilding);
        GameObject newBuildingGO = Instantiate(Data.BuildingPrefab, position, Quaternion.identity);
        BuildingView view = newBuildingGO.GetComponent<BuildingView>();
        view.Init(newBuilding);
        buildingViews[newBuilding.GetID()] = view;

        TerrainController.AdjustTerrainToBuilding(newBuildingGO);

        Debug.Log($"Új építmény lehelyezve, ID {newBuilding.GetID()}");

        return GeneratedID;
    }

    public void RemoveBuilding(int buildingID)
    {
        Building building = activeBuildings.Find(t => t.GetID() == buildingID);
        if (building != null)
        {
            // building.Destroy();
            activeBuildings.Remove(building);
        }

        if (buildingViews.TryGetValue(buildingID, out BuildingView view))
        {
            Vector3Int gridPosition = grid.WorldToCell(view.transform.position);
            Vector3Int flatGridPosition = new Vector3Int(gridPosition.x, 0, gridPosition.z);

            MapData.RemoveObjectAt(flatGridPosition);
            buildingViews.Remove(buildingID);
            TerrainController.RestoreTerrain(view.gameObject);
            Destroy(view.gameObject);
        }


    }

    public void RegrowEvent()
    {
        foreach (var building in activeBuildings)
        {
            if (building.isPlant)
            {
                building.Regrow(UnityEngine.Random.Range(0, building.MaxCapacity));
            }
        }
    }

    public void Refill(int ID)
    {
        var building = activeBuildings.Find(t => t.GetID() == ID);
        if (EconomyManager.Instance.HasEnoughMoney(building.RefillPrice) && building.IsFeeder)
        {
            building.Refill();
            EconomyManager.Instance.AddMoney(building.RefillPrice);
        }
    }

    // Betölti a Resource folderból az összes BuildingData ScriptableObjectet
    private void LoadAllBuildings()
    {
        buildingDatabase = new List<BuildingData>(Resources.LoadAll<BuildingData>("Buildings"));
        Debug.Log($"Betöltve {buildingDatabase.Count} épület.");
    }

    // Generál egy új ID-t
    public int GenerateID()
    {
        nextID++;
        return nextID;
    }

    public void OnNotify(RandomEvent randomEvent)
    {
        // TODO
        if (randomEvent == RandomEvent.REGROW)
        {
            RegrowEvent();
        }
    }

    public bool IsEmpty(Vector3 position)
    {
        Vector3Int gridPosition = grid.WorldToCell(position);
        return !MapData.IsOccupied(new Vector3Int(gridPosition.x, 0, gridPosition.z));
    }
}
