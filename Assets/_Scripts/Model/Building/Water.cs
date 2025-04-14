using System.Collections.Generic;
using UnityEngine;
using static UIComponent;

public class Water : Structure, ISelectable
{
    private DietType dietType;
    public Water(BuildingData Data, int iD) : base(iD, Data.name, Data.icon, Data.type)
    {
        dietType = DietType.Water;
    }

    public Dictionary<UIComponent, object> GetUIData()
    {
        return new Dictionary<UIComponent, object> {
            { Name_text, Name },
            { UIComponent.ID, ID },
            { Sprite_icon, Icon },
        };
    }
}
