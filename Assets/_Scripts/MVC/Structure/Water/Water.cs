using System.Collections.Generic;
using static StructureUIValues;

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

    public Dictionary<StructureUIValues, object> GetUIData()
    {
        return new Dictionary<StructureUIValues, object> {
            { Name_text, Name },
            { StructureUIValues.ID, ID },
            { Sprite_icon, Icon },
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
