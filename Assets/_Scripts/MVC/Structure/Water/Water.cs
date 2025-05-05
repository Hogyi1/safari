using System.Collections.Generic;
using UnityEngine;
using static StructureUIValues;
using static UnityEditor.Experimental.GraphView.Port;

public class Water : Structure, ISelectable, IWaterSource
{
    private DietType dietType;
    public Water(BuildingData Data, int iD) : base(iD, Data.name, Data.icon, Data.type)
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
