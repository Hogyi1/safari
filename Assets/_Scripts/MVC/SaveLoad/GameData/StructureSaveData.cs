using UnityEngine;

/// <summary>
/// Serializable data structure used for saving the state of a structure in the simulation.
/// Supports a variety of types including facilities, feeders, vegetation, and more.
/// </summary>
[System.Serializable]
public class StructureSaveData
{
    /// <summary>
    /// The unique runtime ID of the structure instance.
    /// </summary>
    public int UniqueID;

    /// <summary>
    /// The grid-based position of the structure.
    /// </summary>
    public Vector2Int gridPosition;

    /// <summary>
    /// The type of structure (e.g. Feeder, Facility, Road).
    /// </summary>
    public BuildingType Type;

    // === Common ===

    /// <summary>
    /// Growth stage or health value for structures that regrow (e.g. Vegetation).
    /// </summary>
    public float Stage;

    /// <summary>
    /// Current usable capacity (e.g. food or water available).
    /// </summary>
    public int CurrentCapacity;

    /// <summary>
    /// Maximum possible capacity.
    /// </summary>
    public int MaxCapacity;

    // === Feeder-specific ===

    /// <summary>
    /// The diet type supported by this structure (used in feeders or vegetation).
    /// </summary>
    public DietType DietType;

    // === Facility-specific ===

    /// <summary>
    /// Current upgrade level of the facility.
    /// </summary>
    public int Level;

    /// <summary>
    /// Cost required to upgrade the facility to the next level.
    /// </summary>
    public int UpgradePrice;

    /// <summary>
    /// Cached refill price for feeders (also stored here for flexibility).
    /// </summary>
    public int RefillPrice;

    /// <summary>
    /// Current capacity of the facility (e.g. parking spots, ranger coverage).
    /// </summary>
    public int FacilityCapacity;

    /// <summary>
    /// The manager type this facility is tied to (Vehicle, Ranger, etc.).
    /// </summary>
    public ManagerType FacilityType;

    /// <summary>
    /// Constructs a base save data object for a structure using ID and type only.
    /// Use this constructor and populate remaining fields as needed per structure type.
    /// </summary>
    /// <param name="uniqueID">The runtime-unique identifier of the structure instance.</param>
    /// <param name="type">The structure's category/type (e.g. Road, Facility).</param>
    public StructureSaveData(int uniqueID, BuildingType type)
    {
        UniqueID = uniqueID;
        Type = type;
    }
}
