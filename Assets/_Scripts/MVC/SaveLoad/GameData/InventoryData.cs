using NUnit.Framework;
using UnityEngine;

[System.Serializable]
public class InventoryData
{
    public SerializableDictionary<int, int> items;
    public SerializableDictionary<int, LockState> idPlusState;


    public InventoryData()
    {
        items = new SerializableDictionary<int, int>();
        idPlusState = new SerializableDictionary<int, LockState>();
        items.Clear();
    }
}
