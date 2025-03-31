using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildingManager : MonoBehaviour, IRandomEventObserver
{
    public static BuildingManager Instance;

    private int nextID = 0;

    [SerializeField]
    private List<Building> activeBuildings = new List<Building>();
    public Dictionary<int, BuildingView> buildingViews = new Dictionary<int, BuildingView>();

    private bool isPlacementModeActive = false;

    private Building ActiveBuilding = null;
    private BuildingView ActiveView = null;
    private BuildingView LastView = null;


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

        InputManager.Instance.OnClicked += SetViewActive;
        InputManager.Instance.OnClicked += SetViewInactive;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPlacementModeActive)
        {
            BuildingView hoveredObject = InputManager.Instance.GetHoveredObjectScript();

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
            }
            return;
        }
    }

    public int AddBuilding(BuildingData Data, Vector3 position)
    {
        int GeneratedID = GenerateID();

        Building newBuilding = new Building(GeneratedID, Data);
        activeBuildings.Add(newBuilding);
        BuildingView view = PlacementManager.Instance.PlaceStructure(Data, position, newBuilding);
        buildingViews[newBuilding.GetID()] = view;

        Debug.Log($"Új építmény lehelyezve, ID {newBuilding.GetID()}");

        return GeneratedID;
    }

    public void RemoveBuilding(int buildingID)
    {
        Building building = activeBuildings.Find(t => t.GetID() == buildingID);
        if (building != null)
        {
            activeBuildings.Remove(building);
        }

        if (buildingViews.TryGetValue(buildingID, out BuildingView view))
        {
            PlacementManager.Instance.RemoveStructure(view);
            buildingViews.Remove(buildingID);
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


    /// <summary>
    /// A Viewk kezelése kattintásra
    /// </summary>
    private void SetViewInactive()
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
            SetViewInactive();
            ActiveView = LastView;
            ActiveView.isActive = true;
            ActiveView.ShowUI();
        }
    }
}
