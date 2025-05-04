using System.Collections.Generic;
using UnityEngine;

public class ItemHelper : MonoBehaviour
{
    public static ItemHelper Instance;

    public List<Item> allItems;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        allItems = new List<Item>(Resources.LoadAll<Item>("Items"));
        Instance = this;
    }

    public Item GetItemById(int id)
    {
        return allItems.Find(item => item.id == id);
    }
}
