using System;
using System.Collections.Generic;
using UnityEngine;
using static PopupKeys;
public class Feeder : Structure, ISelectable, IRefillable, IFoodSource
{
    private int Capacity;
    private int MaxCapacity;
    private int RefillPrice;
    private DietType dietType;
    public Feeder(BuildingData Data, int iD) : base(iD, Data.Name, Data.Icon, Data.Type)
    {
        dietType = Data.Diet;
        this.MaxCapacity = Data.Capacity;
        this.Capacity = Data.Capacity / 2;
        this.RefillPrice = Data.Price;
    }

    public Dictionary<PopupKeys, object> GetUIData()
    {
        return new Dictionary<PopupKeys, object> {
            { Name_text, Name },
            { Sprite_icon, Icon },
            { Refill_action, new Action(() => FeederManager.Instance.Refill(ID, CalculateRefillPrice())) },
            { Refill_interact, new Func<bool>(() => {
                int price = CalculateRefillPrice();
                bool canAfford = EconomyManager.Instance.HasEnoughMoney(price);
                return price > 0 && canAfford; }) },
            { Refill_text, new Func<string>(() => {
                int price = CalculateRefillPrice();
                return price > 0 ? "Refill $" + price.ToString() : "Full"; }) },
            { Value_slider, new Func<float>(() => GetCapacity()) },
            { MaxValue_slider, MaxCapacity },
            { Pickup_action, new Action(() => { StructureManager.Instance.RemoveStructure(ID); PopupManager.Instance.HidePopup();})}
        };
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
