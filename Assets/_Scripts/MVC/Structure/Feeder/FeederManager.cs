using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Android;

public class FeederManager : MonoBehaviour, IStructureManager
{
    public static FeederManager Instance;
    private List<Feeder> ActiveFeeders = new List<Feeder>();
    // TODO Ha lesz ötlet akkor egy Feeder nézetet ami változtatja a benne lévő mennyiséget

    //Singleton design
    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    // Létrehozza a megadott Model réteget és eltárolja
    // Visszaadja a Model-t, hogy a fő manager tudjon vele foglalkozni
    public Structure AddStructure(BuildingData Data, int ID, Vector2Int gridPosition)
    {
        Feeder Feeder = new Feeder(Data, ID);
        if (Feeder == null) throw new Exception("Nem sikerült létrehpzni a következőt: Feeder");

        ActiveFeeders.Add(Feeder);

        return Feeder;
    }


    // Törli a saját referenciáját
    public void RemoveStructure(int ID)
    {
        Feeder Feeder = ActiveFeeders.Find(t => t.GetID() == ID);
        if (Feeder == null) return;

        ActiveFeeders.Remove(Feeder);
    }


    public void Refill(int ID, int Price)
    {
        if (EconomyManager.Instance.HasEnoughMoney(Price))
            ActiveFeeders.Find(t => t.GetID() == ID).Refill();
        else Debug.LogWarning("Nincs elegendő pénzed újratölteni!");
    }

    public void SetView(IPlaceable view, int ID)
    {
        return;
    }
}
