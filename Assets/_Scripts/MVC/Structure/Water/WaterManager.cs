using System.Collections.Generic;
using System;
using UnityEngine;

public class WaterManager : MonoBehaviour, IStructureManager
{
    public static WaterManager Instance;
    private List<Water> ActiveWaters = new List<Water>();

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
        Water water = new Water(Data, ID);
        if (water == null) throw new Exception("Nem sikerült léterhozni a következőt: Water");

        ActiveWaters.Add(water);

        Debug.Log("Water placed");
        return water;
    }

    // Törli a saját referenciáját
    public void RemoveStructure(int ID)
    {
        Water water = ActiveWaters.Find(t => t.GetID() == ID);
        if (water == null) return;

        ActiveWaters.Remove(water);
    }

    // Beállítja a megfelelő modellhez a nézetet
    public void SetView(IPlaceable view, int ID) { }
}
