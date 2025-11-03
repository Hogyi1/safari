using System;
using System.Collections.Generic;
using static UIKeys;

/// <summary>
/// Represents a road structure in the simulation. Roads are used for navigation and pathfinding.
/// Implements ISelectable for UI interaction and ISaveable for persistence.
/// </summary>
public class RoadModel : Structure, ISelectable
{
    /// <summary>
    /// Constructs a new road from template building data.
    /// </summary>
    /// <param name="Data">The building data containing metadata about the road.</param>
    /// <param name="iD">The unique ID of this road instance.</param>
    public RoadModel(BuildingData Data, int iD)
        : base(iD, Data.Name, Data.Icon, Data.Type) { }


    /// <summary>
    /// Constructs a road structure from saved data and building template.
    /// </summary>
    /// <param name="data">BuildingData containing static road info.</param>
    /// <param name="saveData">The saved instance state for the road.</param>
    public RoadModel(BuildingData data, StructureSaveData saveData)
        : base(saveData.UniqueID, data.Name, data.Icon, data.Type) { }


    /// <summary>
    /// Returns a dictionary of UI-related data for the popup system.
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
    /// Serializes the road structure's state into a save data object.
    /// </summary>
    public override StructureSaveData GetSaveData()
    {
        return new StructureSaveData(ID, buildingType)
        {
            DietType = DietType.None, // Not used, but required by save structure
        };
    }
}
