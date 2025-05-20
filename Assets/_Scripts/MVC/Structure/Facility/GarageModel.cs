using System;
using System.Collections.Generic;
using static UIKeys;

/// <summary>
/// Represents a parking facility where vehicles are stored or spawned.
/// Inherits from Facility and implements ISelectable for UI interaction.
/// </summary>
public class GarageModel : Facility, ISelectable
{
    /// <summary>
    /// Constructs a new Parking instance from building template data and a generated ID.
    /// </summary>
    /// <param name="Data">The static building data template.</param>
    /// <param name="iD">The unique runtime ID assigned to the instance.</param>
    public GarageModel(BuildingData Data, int iD)
        : base(Data, iD) { }


    /// <summary>
    /// Constructs a Parking instance using save data and template data.
    /// </summary>
    /// <param name="data">The template data (BuildingData).</param>
    /// <param name="saveData">The saved facility state.</param>
    public GarageModel(BuildingData data, StructureSaveData saveData)
        : base(data, saveData) { }


    /// <summary>
    /// Returns a dictionary containing all data required for UI popup generation.
    /// Includes upgrade actions, text, levels, and vehicle-related capacity values.
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
            { Value_slider, new Func<float>(() => VehicleManager.Instance.Capacity) },
            { MaxValue_slider, new Func<float>(() => VehicleManager.Instance.MaxCapacity) }
        };
    }
}
