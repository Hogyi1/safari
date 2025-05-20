using System;
using System.Collections.Generic;
using UnityEngine;
using static UIKeys;

/// <summary>
/// Represents a feeder structure that provides food for animals.
/// Implements ISelectable for UI integration, IRefillable for refill behavior,
/// IFoodSource for consumption by animals, and ISaveable for persistence.
/// </summary>
public class FeederModel : Structure, ISelectable, IRefillable, IFoodSource
{
    /// <summary>
    /// Current amount of food available in the feeder.
    /// </summary>
    private int Capacity;

    /// <summary>
    /// Maximum food capacity the feeder can hold.
    /// </summary>
    private int MaxCapacity;

    /// <summary>
    /// Cost to refill the feeder to full capacity.
    /// </summary>
    private int RefillPrice;

    /// <summary>
    /// Type of diet this feeder supports (e.g., herbivore, carnivore).
    /// </summary>
    private DietType dietType;

    /// <summary>
    /// Constructs a new feeder using static building data and assigns a new ID.
    /// </summary>
    public FeederModel(BuildingData Data, int iD)
        : base(iD, Data.Name, Data.Icon, Data.Type)
    {
        dietType = Data.Diet;
        MaxCapacity = Data.Capacity;
        Capacity = Data.Capacity / 2;
        RefillPrice = Data.Price;
    }


    /// <summary>
    /// Constructs a feeder from saved state and template building data.
    /// </summary>
    /// <param name="data">The original building data used to define the structure.</param>
    /// <param name="saveData">The saved feeder state.</param>
    public FeederModel(BuildingData data, StructureSaveData saveData)
        : base(saveData.UniqueID, data.Name, data.Icon, data.Type)
    {
        dietType = saveData.DietType;
        Capacity = saveData.CurrentCapacity;
        MaxCapacity = saveData.MaxCapacity;
        RefillPrice = saveData.UpgradePrice;
    }


    /// <summary>
    /// Returns the UI elements for displaying feeder information in a popup.
    /// </summary>
    public Dictionary<UIKeys, object> GetUIData()
    {
        return new Dictionary<UIKeys, object> {
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
            { Pickup_action, new Action(() => {
                StructureManager.Instance.RemoveStructure(ID);
                PopupManager.Instance.HidePopup();
            })}
        };
    }


    /// <summary>
    /// Returns the type of diet this feeder supports.
    /// </summary>
    public DietType GetDietType() => dietType;


    /// <summary>
    /// Returns the current food capacity of the feeder.
    /// </summary>
    public int GetCapacity() => Capacity;


    /// <summary>
    /// Returns the maximum food capacity of the feeder.
    /// </summary>
    public int GetMaxCapacity() => MaxCapacity;


    /// <summary>
    /// Fills the feeder to maximum capacity.
    /// </summary>
    public void Refill() => Capacity = MaxCapacity;


    /// <summary>
    /// Calculates the cost to refill the feeder based on how empty it is.
    /// </summary>
    public int CalculateRefillPrice()
    {
        float fillPercentage = (float)Capacity / MaxCapacity;
        float costMultiplier = 1f - fillPercentage;
        int newPrice = (int)(RefillPrice * costMultiplier);
        return newPrice;
    }


    /// <summary>
    /// Consumes a specified amount of food from the feeder.
    /// </summary>
    public int Consume(int amount)
    {
        int consumed = Mathf.Min(amount, Capacity);
        Capacity -= consumed;
        return consumed;
    }


    /// <summary>
    /// Serializes the feeder state into a save data object.
    /// </summary>
    public override StructureSaveData GetSaveData()
    {
        return new StructureSaveData(ID, buildingType)
        {
            DietType = dietType,
            MaxCapacity = MaxCapacity,
            CurrentCapacity = Capacity,
            RefillPrice = RefillPrice,
        };
    }
}
