using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base abstract class for all structures (e.g., feeders, water sources, roads, etc.).
/// Provides shared properties like ID, name, icon, and building type.
/// </summary>
public abstract class Structure : ISaveable<StructureSaveData>
{
    /// <summary>
    /// Unique identifier for the placed structure instance.
    /// </summary>
    protected int ID;

    /// <summary>
    /// Unique identifier that links to the building template (BuildingData).
    /// </summary>
    protected int BuildingID;

    /// <summary>
    /// Display name of the structure.
    /// </summary>
    protected string Name;

    /// <summary>
    /// Type of the structure (e.g., Road, Feeder, Water).
    /// </summary>
    protected BuildingType buildingType;

    /// <summary>
    /// Icon representing the structure in UI.
    /// </summary>
    protected Sprite Icon;

    /// <summary>
    /// Constructs a base structure with common metadata.
    /// </summary>
    protected Structure(int iD, string name, Sprite icon, BuildingType buildingType)
    {
        ID = iD;
        Name = name;
        Icon = icon;
        this.buildingType = buildingType;
    }

    /// <summary>
    /// Returns the unique instance ID of this structure.
    /// </summary>
    public int GetID() => ID;

    /// <summary>
    /// Returns the structure's type.
    /// </summary>
    public BuildingType GetBuildingType() => buildingType;

    /// <summary>
    /// Equality override based on structure ID.
    /// </summary>
    public override bool Equals(object obj)
    {
        return obj is Structure structure &&
               ID == structure.ID;
    }

    /// <summary>
    /// Hash code override based on structure ID.
    /// </summary>
    public override int GetHashCode()
    {
        return ID.GetHashCode();
    }


    /// <summary>
    /// Gets the saving data
    /// </summary>
    /// <returns>new structure save data</returns>
    public virtual StructureSaveData GetSaveData() => new StructureSaveData(ID, buildingType);
}

// ====================== INTERFACES ======================

/// <summary>
/// Interface for structures that can be selected and shown in a UI popup.
/// </summary>
public interface ISelectable
{
    public BuildingType GetBuildingType();
    public int GetID();

    /// <summary>
    /// Returns UI data entries for rendering popups.
    /// </summary>
    public Dictionary<UIKeys, object> GetUIData();
}

/// <summary>
/// Interface for refillable structures (e.g., feeders).
/// </summary>
public interface IRefillable
{
    /// <summary>
    /// Refills the internal resource (e.g., food).
    /// </summary>
    public void Refill();

    /// <summary>
    /// Calculates how much it would cost to refill this structure.
    /// </summary>
    public int CalculateRefillPrice();
}

/// <summary>
/// Interface for food-providing structures (e.g., vegetation, feeders).
/// </summary>
public interface IFoodSource
{
    public DietType GetDietType();
    public int GetCapacity();
    public int GetMaxCapacity();

    /// <summary>
    /// Consumes a specified amount of the food resource.
    /// </summary>
    public int Consume(int amount);
}

/// <summary>
/// Interface for water-providing structures.
/// </summary>
public interface IWaterSource
{
    public DietType GetDietType();

    /// <summary>
    /// Returns whether the water is contaminated (default: false).
    /// </summary>
    public bool IsContaminated();

    /// <summary>
    /// Consumes a specified amount of water.
    /// </summary>
    public int Consume(int amount);
}
