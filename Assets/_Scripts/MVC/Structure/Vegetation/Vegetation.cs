using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Properties;
using UnityEngine;
using static UIComponent;
public class Vegetation : Structure, ISelectable, IFoodSource
{
    private DietType Diet;
    private int Capacity;
    private int MaxCapacity;
    public Vegetation(BuildingData Data, int iD) : base(iD, Data.name, Data.icon, Data.type)
    {
        this.Diet = Data.diet;
        this.Capacity = 0;
        this.MaxCapacity = Data.Capacity;
    }

    public void Regrow(int amount)
    {
        Capacity = (Capacity + amount) >= MaxCapacity ? MaxCapacity : Capacity + amount;
    }

    public int GetCapacity()
    {
        return Capacity;
    }

    public int GetMaxCapacity()
    {
        return MaxCapacity;
    }

    public float GetStage()
    {
        return 1f - (float)Capacity / (float)MaxCapacity;
    }

    public DietType GetDietType()
    {
        return Diet;
    }

    // Minden ami a UI-hoz szükséges adat
    public Dictionary<UIComponent, object> GetUIData()
    {
        return new Dictionary<UIComponent, object> {
            { Name_text, Name },
            { UIComponent.ID, ID },
            { Sprite_icon, Icon },
            { Value_slider, new Func<float>(() => GetCapacity()) },
            { MaxValue_slider, MaxCapacity}
        };
    }
}
