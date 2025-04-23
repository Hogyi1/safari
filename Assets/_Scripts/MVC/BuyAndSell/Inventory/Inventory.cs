using System.Collections.Generic;
using UnityEngine;


public class Inventory : MonoBehaviour {

    //contains the purchesed items
    public List<Item> items;
    public Item currentItem;



    public Inventory Instance;


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
        Item existingItem = items.Find(i => i == item);

        if (existingItem != null)
        {
            existingItem.IncreaseCount(1);
        }
        else
        {
            item.IncreaseCount(1);
            items.Add(item);
        }

        Debug.Log("item hozzáadva");
    }


    private void RemoveItem(Item item)
    {
       items.Remove(item);
    }
    
    
    public void PalaceItem(Item item) {
        DecreaseItemCountOrRemove(item);
    }


    public void DecreaseItemCountOrRemove(Item item)
    {
        item.DecreaseCount(1);
        if (!CanSellItem(item))
        {
            RemoveItem(item);
        }
    }

    public bool HasItem(Item item) { 
       return items.Contains(item);
    }

    public bool CanSellItem(Item item) { 
        return item.Count > 0;
    }


}
