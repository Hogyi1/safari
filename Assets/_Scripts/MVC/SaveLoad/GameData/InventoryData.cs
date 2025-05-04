using UnityEngine;

[System.Serializable]
public class InventoryData
{
    public SerializableDictionary<int, int> items;
    public InventoryData()
    {
        items = new SerializableDictionary<int, int>();
        items.Clear();
    }
}
