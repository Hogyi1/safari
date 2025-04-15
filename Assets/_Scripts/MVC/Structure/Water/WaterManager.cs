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
    public Structure AddStructure(BuildingData Data, int ID)
    {
        Water Water = new Water(Data, ID);
        if (Water == null) throw new Exception("Nem sikerült léterhozni a következőt: Water");

        ActiveWaters.Add(Water);

        return Water;
    }

    // Törli a saját referenciáját
    public void RemoveStructure(int ID)
    {
        Water Water = ActiveWaters.Find(t => t.GetID() == ID);
        if (Water == null) return;

        ActiveWaters.Remove(Water);
    }

    // Beállítja a megfelelő modellhez a nézetet
    public void SetView(IPlaceable view, int ID)
    {
        return;
    }
}
