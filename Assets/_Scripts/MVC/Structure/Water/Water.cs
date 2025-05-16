using System;
using System.Collections.Generic;
using static PopupKeys;

public class Water : Structure, ISelectable, IWaterSource
{
    private DietType dietType;
    public Water(BuildingData Data, int iD) : base(iD, Data.Name, Data.Icon, Data.Type)
    {
        dietType = DietType.Water;
    }

    public DietType GetDietType()
    {
        return dietType;
    }

    public Dictionary<PopupKeys, object> GetUIData()
    {
        return new Dictionary<PopupKeys, object> {
            { Name_text, Name },
            { Sprite_icon, Icon },
            { Pickup_action, new Action(() => { StructureManager.Instance.RemoveStructure(ID); PopupManager.Instance.HidePopup();})}
        };
    }

    public bool IsContaminated()
    {
        // Későbbiekben jó lehet extra featurenek

        return false;
    }

    public int Consume(int amount)
    {
        return amount;
    }
}
