using System.Collections.Generic;

public class Road : Structure, ISelectable
{
    public Road(BuildingData Data, int iD) : base(iD, Data.Name, Data.Icon, Data.Type) { }
    public Dictionary<StructureUIValues, object> GetUIData()
    {
        return new Dictionary<StructureUIValues, object> {
            { StructureUIValues.Name_text, Name },
            { StructureUIValues.ID, ID },
            { StructureUIValues.Sprite_icon, Icon },
        };
    }
}
