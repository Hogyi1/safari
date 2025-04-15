using System.Collections.Generic;
using UnityEngine;
using static UIComponent;
public class Feeder : Structure, ISelectable, IRefillable
{
    private int Capacity;
    private int MaxCapacity;
    private int RefillPrice;
    private DietType dietType;
    public Feeder(BuildingData Data, int iD) : base(iD, Data.name, Data.icon, Data.type)
    {
        dietType = Data.diet;
        this.MaxCapacity = Data.Capacity;
        this.Capacity = Data.Capacity;
        this.RefillPrice = Data.Price;
    }

    public Dictionary<UIComponent, object> GetUIData()
    {
        return new Dictionary<UIComponent, object> {
            { Name_text, Name },
            { UIComponent.ID, ID },
            { Sprite_icon, Icon },
            { Value_slider, Capacity },
            { MaxValue_slider, MaxCapacity},
            { Refillprice_button, CalculateRefillPrice() } };
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
}
