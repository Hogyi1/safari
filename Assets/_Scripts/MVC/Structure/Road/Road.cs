using System;
using System.Collections.Generic;
using static PopupKeys;
public class Road : Structure, ISelectable
{
    public Road(BuildingData Data, int iD) : base(iD, Data.Name, Data.Icon, Data.Type) { }
    public Dictionary<PopupKeys, object> GetUIData()
    {
        return new Dictionary<PopupKeys, object> {
            { Name_text, Name },
            { Sprite_icon, Icon },
            { Pickup_action, new Action(() => { StructureManager.Instance.RemoveStructure(ID); PopupManager.Instance.HidePopup();})}
        };
    }
}
