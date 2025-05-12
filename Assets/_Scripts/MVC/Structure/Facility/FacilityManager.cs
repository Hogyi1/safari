using System.Collections.Generic;
using System;
using UnityEngine;

public class FacilityManager : MonoBehaviour, IStructureManager
{
    public static FacilityManager Instance;
    private List<Facility> activeFacilities = new List<Facility>();
    private Dictionary<int, FacilityView> activeViews = new Dictionary<int, FacilityView>();

    [SerializeField] private int garageUpgradeAmount = 2;
    [SerializeField] private int rangerUpgradeAmount = 1;
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

    // Létrehozza a megadott Model réteget és eltárolja
    // Visszaadja a Model-t, hogy a fő manager tudjon vele foglalkozni
    public Structure AddStructure(BuildingData Data, int ID, Vector2Int gridPosition)
    {
        Facility facility = null;
        switch (Data.ToUpgrade)
        {
            case ManagerType.Vehicle:
                facility = new Parking(Data, ID);
                break;
            case ManagerType.Ranger:
                facility = new RangerHouse(Data, ID);
                break;
            default:
                break;

        }
        if (facility == null) throw new Exception("Nem sikerült léterhozni a következőt: Facility");

        activeFacilities.Add(facility);

        Debug.Log("Facility placed");
        return facility;
    }

    // Törli a saját referenciáját
    public void RemoveStructure(int ID)
    {
        Facility facility = activeFacilities.Find(t => t.GetID() == ID);
        if (facility == null) return;

        activeFacilities.Remove(facility);
    }

    // Beállítja a megfelelő modellhez a nézetet
    public void SetView(IPlaceable view, int ID)
    {
        activeViews[ID] = (FacilityView)view;
    }

    public bool AtMaxLevel(int ID)
    {
        return !activeFacilities.Find(t => t.GetID() == ID).CanUpgrade();
    }

    public void HandleUpgrade(int ID, int price)
    {
        Facility upgrade = GetFacilityByID(ID);

        switch (upgrade.ToUpgrade)
        {
            case ManagerType.Vehicle:
                upgrade.LevelUp(garageUpgradeAmount);
                activeViews[ID].LevelUp(0);
                VehicleManager.Instance.LevelUp(garageUpgradeAmount);
                break;
            case ManagerType.Ranger:
                upgrade.LevelUp(rangerUpgradeAmount);
                activeViews[ID].LevelUp(0);
                RangerManager.Instance.LevelUp(rangerUpgradeAmount);
                break;
        }

        EconomyManager.Instance.RemoveMoney(price);
    }

    public Vector3 GetInteractingPosition(ManagerType facilityType)
    {
        Vector3 position = Vector3.zero;

        Facility facility = GetFacilityByType(facilityType);
        if (facility == null) return position;

        return activeViews[facility.GetID()].GetInteractingPosition();
    }

    private Facility GetFacilityByID(int ID)
    {
        return activeFacilities.Find(t => t.GetID() == ID);
    }
    private Facility GetFacilityByType(ManagerType type)
    {
        return activeFacilities.Find(t => t.ToUpgrade == type);
    }

}

public enum ManagerType
{
    None,
    Vehicle,
    Ranger,
}