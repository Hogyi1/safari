using System.Collections.Generic;
using UnityEngine;

///// <summary>
///// Represents a simple inventory system storing item counts using a dictionary.
///// Provides m
public class Inventory
{
    /// <summary>
    /// Dictionary mapping items to their quantity in inventory.
    /// </summary>
    private Dictionary<Item, int> items;

    /// <summary>
    /// Creates an empty inventory.
    /// </summary>
    public Inventory()
    {
        items = new Dictionary<Item, int>();
    }

    /// <summary>
    /// Creates an inventory with an initial item set.
    /// </summary>
    /// <param name="items">Initial item dictionary.</param>
    public Inventory(Dictionary<Item, int> items)
    {
        this.items = items;
    }

    /// <summary>
    /// Adds one instance of the given item to the inventory.
    /// </summary>
    /// <param name="item">The item to add.</param>
    public void AddItem(Item item)
    {
        if (items.ContainsKey(item))
            items[item]++;
        else
            items[item] = 1;
    }

    /// <summary>
    /// Removes one instance of the given item from the inventory.
    /// If the count reaches zero, the item is removed entirely.
    /// </summary>
    /// <param name="item">The item to remove.</param>
    public void RemoveItem(Item item)
    {
        if (!items.TryGetValue(item, out int count)) return;

        if (count <= 1)
            items.Remove(item);
        else
            items[item] = count - 1;
    }
    /// <summary>
    /// Decreases the count of the given item or removes it entirely.
    /// Can be used for sale or consumption logic.
    /// </summary>
    /// <param name="item">The item to decrease or remove.</param>
    public void DecreaseItemCountOrRemove(Item item)
    {
        if (!items.ContainsKey(item)) return;

        items[item]--;

        if (items[item] <= 0)
            items.Remove(item);
    }

    /// <summary>
    /// Checks whether the inventory contains the given item.
    /// </summary>
    /// <param name="item">The item to check.</param>
    /// <returns>True if the item exists in inventory.</returns>
    public bool HasItem(Item item) => items.ContainsKey(item);

    /// <summary>
    /// Returns true if the item is present and can be sold (quantity > 0).
    /// </summary>
    /// <param name="item">The item to check.</param>
    /// <returns>True if the item can be sold.</returns>
    public bool CanSellItem(Item item) => GetItemCount(item) > 0;

    /// <summary>
    /// Gets the current quantity of the given item in the inventory.
    /// </summary>
    /// <param name="item">The item to count.</param>
    /// <returns>Number of instances, or 0 if not present.</returns>
    public int GetItemCount(Item item) => items.TryGetValue(item, out int count) ? count : 0;
}