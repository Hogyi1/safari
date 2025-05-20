using System;
using System.Collections.Generic;
using static UIKeys;

/// <summary>
/// Represents a water source structure in the simulation. Provides hydration for animals.
/// Implements ISelectable for UI and IWaterSource for interaction with animal AI.
/// </summary>
public class WaterModel : Structure, ISelectable, IWaterSource
{
    private DietType dietType;


    /// <summary>
    /// Constructs a new water structure from building data and assigns a unique ID.
    /// </summary>
    public WaterModel(BuildingData Data, int iD) : base(iD, Data.Name, Data.Icon, Data.Type)
    {
        dietType = DietType.Water;
    }


    /// <summary>
    /// Constructs a water structure from saved data and building template data.
    /// </summary>
    public WaterModel(BuildingData data, StructureSaveData saveData)
        : base(saveData.UniqueID, data.Name, data.Icon, data.Type)
    {
        dietType = DietType.Water;
    }


    /// <summary>
    /// Returns the diet type that this water source supports (always DietType.Water).
    /// </summary>
    public DietType GetDietType() => dietType;


    /// <summary>
    /// Indicates whether the water source is contaminated. Always false for now.
    /// </summary>
    public bool IsContaminated() => false;


    /// <summary>
    /// Simulates consuming water from the source. Always returns the requested amount.
    /// </summary>
    public int Consume(int amount) => amount;


    /// <summary>
    /// Returns a dictionary of UI elements and data for the popup system.
    /// </summary>
    public Dictionary<UIKeys, object> GetUIData()
    {
        return new Dictionary<UIKeys, object> {
            { Name_text, Name },
            { Sprite_icon, Icon },
            { Pickup_action, new Action(() => {
                StructureManager.Instance.RemoveStructure(ID);
                PopupManager.Instance.HidePopup();
            })}
        };
    }


    /// <summary>
    /// Serializes the water structure into a save data object.
    /// </summary>
    public override StructureSaveData GetSaveData()
    {
        return new StructureSaveData(ID, buildingType)
        {
            DietType = dietType,
        };
    }
}
