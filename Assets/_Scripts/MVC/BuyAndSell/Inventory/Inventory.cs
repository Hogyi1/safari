using System.Collections.Generic;
using UnityEngine;


public class Inventory : MonoBehaviour {

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

   
}
