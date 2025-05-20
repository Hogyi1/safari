using System;
using System.Collections.Generic;
using UnityEngine;
using static UIKeys;

/// <summary>
/// Represents a natural vegetation structure that acts as a food source for herbivores.
/// Implements ISelectable for UI popup support and IFoodSource for animal consumption.
/// </summary>
public class VegetationModel : Structure, ISelectable, IFoodSource
{
    /// <summary>
    /// The diet type this vegetation supports (typically herbivore).
    /// </summary>
    private DietType Diet;

    /// <summary>
    /// Current food capacity of the vegetation.
    /// </summary>
    private int Capacity;

    /// <summary>
    /// Maximum capacity the vegetation can store.
    /// </summary>
    private int MaxCapacity;


    /// <summary>
    /// Constructs a new vegetation instance from template data.
    /// </summary>
    public VegetationModel(BuildingData Data, int iD)
        : base(iD, Data.Name, Data.Icon, Data.Type)
    {
        this.Diet = Data.Diet;
        this.Capacity = 0;
        this.MaxCapacity = Data.Capacity;
    }


    /// <summary>
    /// Constructs a vegetation structure from saved state and building template.
    /// </summary>
    /// <param name="data">The associated BuildingData template.</param>
    /// <param name="saveData">The saved structure data.</param>
    public VegetationModel(BuildingData data, StructureSaveData saveData)
        : base(saveData.UniqueID, data.Name, data.Icon, data.Type)
    {
        Diet = saveData.DietType;
        Capacity = saveData.CurrentCapacity;
        MaxCapacity = saveData.MaxCapacity;
    }


    /// <summary>
    /// Gets the current food capacity.
    /// </summary>
    public int GetCapacity() => Capacity;


    /// <summary>
    /// Gets the diet type this vegetation serves.
    /// </summary>
    public DietType GetDietType() => Diet;


    /// <summary>
    /// Gets the maximum food capacity.
    /// </summary>
    public int GetMaxCapacity() => MaxCapacity;


    /// <summary>
    /// Regrows the food capacity by a given amount (up to max).
    /// </summary>
    public void Regrow(int amount)
    {
        Capacity = (Capacity + amount) >= MaxCapacity ? MaxCapacity : Capacity + amount;
    }


    /// <summary>
    /// Gets the stage of regrowth as a normalized 0–1 value (used for visuals).
    /// </summary>
    public float GetStage()
    {
        return 1f - (float)Capacity / (float)MaxCapacity;
    }


    /// <summary>
    /// Consumes a specific amount of food, reducing internal capacity.
    /// </summary>
    public int Consume(int amount)
    {
        int consumed = Mathf.Min(amount, Capacity);
        Capacity -= consumed;
        return consumed;
    }


    /// <summary>
    /// Returns UI data for rendering vegetation info in popup panels.
    /// </summary>
    public Dictionary<UIKeys, object> GetUIData()
    {
        return new Dictionary<UIKeys, object> {
            { Name_text, Name },
            { Sprite_icon, Icon },
            { Value_slider, new Func<float>(() => GetCapacity()) },
            { MaxValue_slider, MaxCapacity },
            { Pickup_action, new Action(() => {
                StructureManager.Instance.RemoveStructure(ID);
                PopupManager.Instance.HidePopup();
            })}
        };
    }


    /// <summary>
    /// Serializes the vegetation state into a save data structure.
    /// </summary>
    public override StructureSaveData GetSaveData()
    {
        return new StructureSaveData(ID, buildingType)
        {
            DietType = Diet,
            MaxCapacity = this.MaxCapacity,
            CurrentCapacity = Capacity,
        };
    }
}
