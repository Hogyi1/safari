using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public long lastUpdated;
    public int exempleint;
    public SerializableDictionary<int, int> exemple;
    public ParkData parkData;
    public InventoryData inventoryData;

    // the values defined in this constructor will be the default values
    // the game starts with when there's no data to load
    public GameData()
    {
        //This is where you need to add the data, according to how you want your class to be initialized
        this.exemple = new SerializableDictionary<int, int>();
        this.parkData = new ParkData();
        this.inventoryData = new InventoryData();
    }

    public int GetPercentageComplete()
    {
        return 0;
    }
}