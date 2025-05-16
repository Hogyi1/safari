using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Properties;
using UnityEngine;
using static PopupKeys;
public class Vegetation : Structure, ISelectable, IFoodSource
{
    private DietType Diet;
    private int Capacity;
    private int MaxCapacity;
    public Vegetation(BuildingData Data, int iD) : base(iD, Data.Name, Data.Icon, Data.Type)
    {
        this.Diet = Data.Diet;
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
    public int Consume(int amount)
    {
        int consumed = Mathf.Min(amount, Capacity);
        Capacity -= consumed;
        return consumed;
    }

    // Minden ami a UI-hoz szükséges adat
    public Dictionary<PopupKeys, object> GetUIData()
    {
        return new Dictionary<PopupKeys, object> {
            { Name_text, Name },
            { Sprite_icon, Icon },
            { Value_slider, new Func<float>(() => GetCapacity()) },
            { MaxValue_slider, MaxCapacity },
            { Pickup_action, new Action(() => { StructureManager.Instance.RemoveStructure(ID); PopupManager.Instance.HidePopup();})}
        };
    }
}
