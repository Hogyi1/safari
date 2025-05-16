using System;
using System.Collections.Generic;
using static UIKeys;
public class Road : Structure, ISelectable
{
    public Road(BuildingData Data, int iD) : base(iD, Data.Name, Data.Icon, Data.Type) { }
    public Dictionary<UIKeys, object> GetUIData()
    {
        return new Dictionary<UIKeys, object> {
            { Name_text, Name },
            { Sprite_icon, Icon },
            { Pickup_action, new Action(() => { StructureManager.Instance.RemoveStructure(ID); PopupManager.Instance.HidePopup();})}
        };
    }
}
