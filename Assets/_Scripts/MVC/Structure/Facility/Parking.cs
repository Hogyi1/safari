using System;
using System.Collections.Generic;
using static UIKeys;

public class Parking : Facility, ISelectable
{
    public Parking(BuildingData Data, int iD) : base(Data, iD) { }

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
                return price > 0 && canAfford && canUpgrade; }) },
            { Upgrade_text, new Func<string>(() => {
                int price = GetUpgradePrice();
                bool canUpgrade = !FacilityManager.Instance.AtMaxLevel(ID);
                return canUpgrade ? "Upgrade $" + price.ToString() : "Max level"; }) },
            { Level_text, new Func<string>(() => GetCurrentLevel().ToString()) },
            { Value_slider, new Func<float>(() => VehicleManager.Instance.Capacity) },
            { MaxValue_slider, new Func<float>(() => VehicleManager.Instance.MaxCapacity) }
        };
    }
}
