using System.Collections.Generic;
using UnityEngine;

public class Inventory
{

    //contains the purchesed items
    [SerializeField] public SerializableDictionary<Item, int> items;
    public Item currentItem;


    public Inventory()
    {
        items = new SerializableDictionary<Item, int>();
    }

    //a saveing miatt kell
    public Inventory(SerializableDictionary<Item, int> items)
    {
        this.items = items;
    }
    public void AddItem(Item item)
    {

        if (items.TryGetValue(item, out int itemcount))
        {
            items[item] += 1;
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

    public bool HasItem(Item item)
    {
        return items.ContainsKey(item) && items[item] > 0;
    }

    public bool CanSellItem(Item item)
    {
        return GetItemCount(item) > 0;
    }

    public int GetItemCount(Item item)
    {
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
