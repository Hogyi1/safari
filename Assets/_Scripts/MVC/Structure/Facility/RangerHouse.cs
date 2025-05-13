using System;
using System.Collections.Generic;
using static StructureUIValues;

public class RangerHouse : Facility, ISelectable
{
    public RangerHouse(BuildingData Data, int iD) : base(Data, iD) { }

    public Dictionary<StructureUIValues, object> GetUIData()
    {
        return new Dictionary<StructureUIValues, object> {
            { Name_text, Name },
            { StructureUIValues.ID, ID },
            { Sprite_icon, Icon },
            { Upgradeprice_button, new Func<float>(() => GetUpgradePrice()) },
            { Level, new Func<int>(() => GetCurrentLevel()) },
            { Value_slider, new Func<float>(() => RangerManager.Instance.Capacity) },
            { MaxValue_slider, new Func<float>(() => RangerManager.Instance.MaxCapacity) }
        };
    }
}
