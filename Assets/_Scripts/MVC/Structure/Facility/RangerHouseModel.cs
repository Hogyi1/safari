using System;
using System.Collections.Generic;
using static UIKeys;

/// <summary>
/// Represents a ranger house facility that expands ranger-related functionality,
/// such as wildlife monitoring or resource management. Inherits from Facility
/// and supports UI interactions via ISelectable.
/// </summary>
public class RangerHouseModel : Facility, ISelectable
{
    /// <summary>
    /// Constructs a new RangerHouse instance from template data and a generated ID.
    /// </summary>
    /// <param name="Data">The static building data template.</param>
    /// <param name="iD">The unique runtime ID for this instance.</param>
    public RangerHouseModel(BuildingData Data, int iD)
        : base(Data, iD) { }


    /// <summary>
    /// Constructs a RangerHouse instance from saved data and building template.
    /// </summary>
    /// <param name="data">The original building definition (BuildingData).</param>
    /// <param name="saveData">Previously saved runtime state of the facility.</param>
    public RangerHouseModel(BuildingData data, StructureSaveData saveData)
        : base(data, saveData) { }


    /// <summary>
    /// Returns a dictionary of UI data used to populate the popup panel.
    /// Contains upgrade logic, current level, and slider values related to ranger capacity.
    /// </summary>
    public Dictionary<UIKeys, object> GetUIData()
    {
        return new Dictionary<UIKeys, object> {
            { Name_text, Name },
            { Sprite_icon, Icon },
            { Upgrade_action, new Action(() => FacilityManager.Instance.HandleUpgrade(ID, GetUpgradePrice())) },
            { Upgrade_interact, new Func<bool>(() => {
                int price = GetUpgradePrice();
                bool canAfford = EconomyManager.Instance.HasEnoughMoney(price);
                bool canUpgrade = !FacilityManager.Instance.AtMaxLevel(ID);
                return price > 0 && canAfford && canUpgrade;
            }) },
            { Upgrade_text, new Func<string>(() => {
                int price = GetUpgradePrice();
                bool canUpgrade = !FacilityManager.Instance.AtMaxLevel(ID);
                return canUpgrade ? "Upgrade $" + price.ToString() : "Max level";
            }) },
            { Level_text, new Func<string>(() => GetCurrentLevel().ToString()) },
            { Value_slider, new Func<float>(() => RangerManager.Instance.Capacity) },
            { MaxValue_slider, new Func<float>(() => RangerManager.Instance.MaxCapacity) }
        };
    }
}
