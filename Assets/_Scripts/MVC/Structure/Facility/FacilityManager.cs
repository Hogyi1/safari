using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages all facility-type structures in the game (e.g., Parking, RangerHouse).
/// Handles creation, registration from save, upgrade logic, and view management.
/// </summary>
public class FacilityManager : MonoBehaviour, IStructureManager
{
    /// <summary>
    /// Singleton instance of the FacilityManager.
    /// </summary>
    public static FacilityManager Instance;

    [Header("Active Facilities")]
    /// <summary>
    /// List of all active Facility models.
    /// </summary>
    private List<Facility> activeFacilities = new();

    /// <summary>
    /// Dictionary mapping Facility IDs to their corresponding views.
    /// </summary>
    private Dictionary<int, FacilityView> activeViews = new();


    /// <summary>
    /// Ensures Singleton pattern and prevents duplicate instances.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    /// <summary>
    /// Creates and registers a new facility based on provided data and ID.
    /// Also updates the corresponding upgrade manager’s capacity.
    /// </summary>
    /// <param name="data">The building data for the facility.</param>
    /// <param name="ID">The unique structure ID.</param>
    /// <param name="gridPosition">The position on the grid (not used here).</param>
    /// <returns>The created Facility instance as a Structure.</returns>
    public Structure AddStructure(BuildingData data, int ID, Vector2Int gridPosition)
    {
        Facility facility = CreateFacility(data, ID);
        activeFacilities.Add(facility);
        GetManagerByType(data.ToUpgrade).SetCapacity(facility.Capacity);
        Debug.Log("Facility placed");
        return facility;
    }


    /// <summary>
    /// Restores a facility from save data and updates the upgrade manager's capacity.
    /// </summary>
    /// <param name="data">The building data.</param>
    /// <param name="saveData">The structure's saved state.</param>
    /// <param name="gridPosition">The grid position (not used here).</param>
    /// <returns>The reconstructed Facility instance.</returns>
    public Structure RegisterStructure(BuildingData data, StructureSaveData saveData, Vector2Int gridPosition)
    {
        Facility facility = CreateFacility(data, saveData);
        activeFacilities.Add(facility);
        GetManagerByType(data.ToUpgrade).SetCapacity(saveData.FacilityCapacity);
        Debug.Log("Facility placed");
        return facility;
    }


    /// <summary>
    /// Instantiates a Facility from building data and an ID.
    /// </summary>
    private Facility CreateFacility(BuildingData data, int ID)
    {
        return data.ToUpgrade switch
        {
            ManagerType.Vehicle => new GarageModel(data, ID),
            ManagerType.Ranger => new RangerHouseModel(data, ID),
            _ => throw new Exception("Unknown Facility Type")
        };
    }


    /// <summary>
    /// Instantiates a Facility from building data and saved data.
    /// </summary>
    private Facility CreateFacility(BuildingData data, StructureSaveData saveData)
    {
        return data.ToUpgrade switch
        {
            ManagerType.Vehicle => new GarageModel(data, saveData),
            ManagerType.Ranger => new RangerHouseModel(data, saveData),
            _ => throw new Exception("Unknown Facility Type")
        };
    }


    /// <summary>
    /// Removes a facility and its references by ID.
    /// </summary>
    /// <param name="ID">The facility's unique ID.</param>
    public void RemoveStructure(int ID)
    {
        Facility facility = activeFacilities.Find(t => t.GetID() == ID);
        if (facility == null) return;

        activeFacilities.Remove(facility);
    }


    /// <summary>
    /// Sets the view reference for a facility by ID.
    /// </summary>
    /// <param name="view">The view to link to the facility.</param>
    /// <param name="ID">The facility ID.</param>
    public void SetView(IPlaceable view, int ID) => activeViews[ID] = (FacilityView)view;


    /// <summary>
    /// Checks if a facility has reached its maximum upgrade level.
    /// </summary>
    /// <param name="ID">The facility ID.</param>
    /// <returns>True if at max level, false otherwise.</returns>
    public bool AtMaxLevel(int ID) => !activeFacilities.Find(t => t.GetID() == ID).CanUpgrade();


    /// <summary>
    /// Handles upgrading a facility and its related manager. Deducts money and grants EXP.
    /// </summary>
    /// <param name="ID">The facility ID.</param>
    /// <param name="price">The cost of the upgrade.</param>
    public void HandleUpgrade(int ID, int price)
    {
        Facility upgrade = GetFacilityByID(ID);

        var manager = GetManagerByType(upgrade.ToUpgrade);
        if (manager == null) return;

        upgrade.LevelUp(upgrade.UpgradeAmount);
        activeViews[ID].LevelUp(0);
        manager.LevelUp(upgrade.UpgradeAmount);

        GameEvents.Instance.NotifyObservers(EventType.EXP_GAIN, 40);
        EconomyManager.Instance.RemoveMoney(price);
    }


    /// <summary>
    /// Returns the spawn position associated with a facility type.
    /// </summary>
    /// <param name="facilityType">The type of the facility.</param>
    /// <returns>The spawn position in world space.</returns>
    public Vector3 GetSpawnPosition(ManagerType facilityType)
    {
        Facility facility = GetFacilityByType(facilityType);
        if (facility == null) return Vector3.zero;
        return activeViews[facility.GetID()].GetSpawnPosition();
    }


    /// <summary>
    /// Returns the interacting position for the facility type.
    /// </summary>
    /// <param name="facilityType">The facility type to query.</param>
    /// <returns>The world position where interaction occurs.</returns>
    public Vector3 GetInteractingPosition(ManagerType facilityType)
    {
        Facility facility = GetFacilityByType(facilityType);
        if (facility == null) return Vector3.zero;
        return activeViews[facility.GetID()].GetInteractingPosition();
    }


    /// <summary>
    /// Finds a facility by its ID.
    /// </summary>
    private Facility GetFacilityByID(int ID) => activeFacilities.Find(t => t.GetID() == ID);


    /// <summary>
    /// Finds a facility based on its upgrade type.
    /// </summary>
    private Facility GetFacilityByType(ManagerType type) => activeFacilities.Find(t => t.ToUpgrade == type);


    /// <summary>
    /// Returns the appropriate upgradeable manager instance for the specified type.
    /// </summary>
    /// <param name="type">The ManagerType to resolve.</param>
    /// <returns>The IUpgradeable manager instance, or null if unknown.</returns>
    public static IUpgradeable GetManagerByType(ManagerType type)
    {
        return type switch
        {
            ManagerType.Vehicle => VehicleManager.Instance,
            ManagerType.Ranger => RangerManager.Instance,
            _ => null
        };
    }
}

/// <summary>
/// Represents the type of manager responsible for a specific Facility.
/// Used to determine which system handles upgrades and interactions.
/// </summary>
public enum ManagerType
{
    /// <summary>
    /// No associated manager (default or unassigned).
    /// </summary>
    None,

    /// <summary>
    /// Managed by the VehicleManager (e.g., Parking facilities).
    /// </summary>
    Vehicle,

    /// <summary>
    /// Managed by the RangerManager (e.g., Ranger Houses).
    /// </summary>
    Ranger,
}
