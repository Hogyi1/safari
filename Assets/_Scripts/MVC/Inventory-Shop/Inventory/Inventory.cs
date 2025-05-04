using System.Collections.Generic;
using UnityEngine;


public class Inventory : MonoBehaviour, IDataPersistence
{

    //contains the purchesed items
    public static Inventory Instance;
    [SerializeField]public Dictionary<Item, int> items = new ();
    public Item currentItem;


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

    public void AddItem(Item item)
    {

        if (items.TryGetValue(item, out int itemcount))
        {
            items[item] +=  1;
        }
        else
        {
            items.Add(item, 1);
        }
        Debug.Log("item hozzáadva");
    }


    public void RemoveItem(Item item)
    {
        if (items.TryGetValue(item, out int itemcount))
        {
            itemcount -= 1;
            if (itemcount <= 0)
            {
                items.Remove(item); 
            }
            else
            {
                items[item] = itemcount; 
            }
        }

    }
    
   
    public void DecreaseItemCountOrRemove(Item item)
    {
        if (!CanSellItem(item))
        {
            RemoveItem(item);
        }
        else
        {
            if (items.ContainsKey(item))
            {
                items[item] -= 1;

                if (items[item] <= 0)
                {
                    items.Remove(item); 
                }
            }
        }
    }

    public bool HasItem(Item item) {
        return items.ContainsKey(item) && items[item] > 0;
    }


    public bool CanSellItem(Item item) { 
        return GetItemCount(item) > 0;
    }


    public int GetItemCount(Item item) {
        if (items.TryGetValue(item, out int itemcount))
        {
            return itemcount;
        }
        else
        {
            return 0;
        }

    }

    public void LoadData(GameData data)
    {
        items.Clear();

        if (data.inventoryData == null || data.inventoryData.items == null)
            return;
        Debug.Log("asd");
        foreach (var pair in data.inventoryData.items)
        {
            Item item = ItemHelper.Instance.GetItemById(pair.Key);
            if (item != null)
            {
                items[item] = pair.Value;
            }
            else
            {
                Debug.LogWarning($"Item with ID {pair.Key} not found in ItemHelper.");
            }
        }

        Debug.Log("Inventory betöltve.");
    }
    public void SaveData(GameData data)
    {
        data.inventoryData.items.Clear();

        foreach (var pair in items)
        {
            data.inventoryData.items[pair.Key.id] = pair.Value;
        }

        Debug.Log("Inventory elmentve.");
    }
}
