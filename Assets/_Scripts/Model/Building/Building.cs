using System;
using UnityEngine;

public class Building
{
    public int Capacity { get; set; }
    public int MaxCapacity { get; private set; }
    public bool HasCapacity { get; private set; } = false;
    public bool isFeeder;
    public DietType ForAnimalType;
    public BuildingType type;
    public int RefillPrice;
    public string name;
    private int ID;

    public bool isPlant { get; }

    public Building(int id, BuildingData Data)
    {
        this.ID = id;
        this.type = Data.type;
        isPlant = Data.isPlant;
        MaxCapacity = Data.Capacity;
        Capacity = Data.Capacity;
        isFeeder = Data.isFeeder;
        RefillPrice = Data.Price;
        name = type.ToString();
    }

    private void Init()
    {

    }

    public string GetCapacityString()
    {
        return Capacity + "/" + MaxCapacity;
    }

    public void Regrow(int amount)
    {
        Capacity = (Capacity + amount) < MaxCapacity ? Capacity + amount : MaxCapacity;
        HasCapacity = true;
    }

    public bool DecreaseCapacity(int amount)
    {
        if (Capacity >= amount)
        {
            Capacity -= amount;
            HasCapacity = Capacity > 0;
            return true;
        }
        return false;
    }

    public void Refill()
    {
        Capacity = MaxCapacity;
    }

    public DietType GetDietType()
    {
        return ForAnimalType;
    }

    public int GetID()
    {
        return ID;
    }

    internal void Destroy()
    {
        throw new NotImplementedException();
    }

    public BuildingType GetBuildingType()
    {
        return type;
    }
}
