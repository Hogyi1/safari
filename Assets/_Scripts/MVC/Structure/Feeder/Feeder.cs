using System;
using System.Collections.Generic;
using UnityEngine;
using static StructureUIValues;
public class Feeder : Structure, ISelectable, IRefillable, IFoodSource
{
    private int Capacity;
    private int MaxCapacity;
    private int RefillPrice;
    private DietType dietType;
    public Feeder(BuildingData Data, int iD) : base(iD, Data.name, Data.icon, Data.type)
    {
        dietType = Data.diet;
        this.MaxCapacity = Data.Capacity;
        this.Capacity = Data.Capacity / 2;
        this.RefillPrice = Data.Price;
    }

    public Dictionary<StructureUIValues, object> GetUIData()
    {
        return new Dictionary<StructureUIValues, object> {
            { Name_text, Name },
            { StructureUIValues.ID, ID },
            { Sprite_icon, Icon },
            { Value_slider, new Func<float>(() => GetCapacity()) },
            { MaxValue_slider, MaxCapacity},
            { Refillprice_button, new Func<float>(() => CalculateRefillPrice()) } };
    }

    public void Refill()
    {
        Capacity = MaxCapacity;
    }

    public int CalculateRefillPrice()
    {
        float fillPercentage = (float)Capacity / MaxCapacity;
        float costMultiplier = 1f - fillPercentage;

        int newPrice = (int)(RefillPrice * costMultiplier);
        return newPrice;
    }

    public DietType GetDietType()
    {
        return dietType;
    }

    public int GetCapacity()
    {
        return Capacity;
    }

    public int GetMaxCapacity()
    {
        return MaxCapacity;
    }
    public int Consume(int amount)
    {
        int consumed = Mathf.Min(amount, Capacity);
        Capacity -= consumed;
        return consumed;
    }
}
