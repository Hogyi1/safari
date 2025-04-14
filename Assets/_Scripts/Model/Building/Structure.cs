using System.Collections.Generic;
using UnityEngine;

public abstract class Structure
{
    protected int ID;
    protected string Name;
    protected BuildingType buildingType;
    protected Sprite Icon;

    protected Structure(int iD, string name, Sprite icon, BuildingType buildingType)
    {
        ID = iD;
        Name = name;
        Icon = icon;
        this.buildingType = buildingType;
    }

    public int GetID()
    {
        return ID;
    }
    public BuildingType GetBuildingType()
    {
        return buildingType;
    }

    public override bool Equals(object obj)
    {
        return obj is Structure structure &&
               ID == structure.ID;
    }
    public override int GetHashCode()
    {
        return ID.GetHashCode();
    }
}


// Interfész ISelectable
// Minden popup-hoz szükséges adattal rendelkező építmény megvalósítja.
public interface ISelectable
{
    public BuildingType GetBuildingType();
    public int GetID();
    public Dictionary<UIComponent, object> GetUIData();
}

//Interfész IRefillable
//Minden, aminek újratölthető funkciója van, megvalósítja pl: Feeder
public interface IRefillable
{
    public void Refill();
    public int CalculateRefillPrice();
}

//Interfész IUpgradeable
//Minden, aminek fejleszthető funkciója van megvalósítja pl: Vadőrház
public interface IUpgradeable
{
    public void Upgrade();
}

//Interfész IFoodSource
//Minden, ami ehető pl: Feeder vagy Növény megvalósítja
public interface IFoodSource
{
    public DietType GetDietType();
    public int GetCapacity();
    public int GetMaxCapacity();
}
