using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static LockState;

/// <summary>
/// Manages all item-related logic, including loading, unlocking, and retrieval of items.
/// This class is implemented as a singleton and persists across scenes.
/// </summary>
public class ItemManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the ItemManager.
    /// </summary>
    public static ItemManager Instance;

    /// <summary>
    /// Stores all items currently loaded into the system.
    /// </summary>
    private HashSet<Item> Items;

    /// <summary>
    /// Initializes the singleton instance and loads all items from Resources.
    /// Ensures only one instance exists and persists across scenes.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadAllItems();
    }

    /// <summary>
    /// Returns all currently loaded items as a List.
    /// </summary>
    /// <returns>List of all loaded Item instances.</returns>
    public List<Item> GetItems() => Items.ToList();

    /// <summary>
    /// Unlocks all items in the collection that match the given item ID and are currently locked.
    /// </summary>
    /// <param name="itemID">The ID of the item(s) to unlock.</param>
    public void UnlockItem(int itemID)
    {
        Items.ToList()
            .FindAll(t => t.ID == itemID && t.LockState == Locked)
            .ForEach(t => t.LockState = Unlocked);
    }

    /// <summary>
    /// Loads all Item assets located under the "Resources/Items" folder into memory.
    /// </summary>
    private void LoadAllItems()
    {
        Items = new HashSet<Item>(Resources.LoadAll<Item>("Items"));
        Debug.Log($"Betöltve {Items.Count} Item.");
    }
}

/// <summary>
/// Enum representing different item categories.
/// Used to classify items by type.
/// </summary>
public enum Category
{
    Structure, // 0
    Vegetation, // 1
    Animal, // 2
    Vehicle, // 3
    Service, // 4
}

/// <summary>
/// Enum representing the lock state of an item.
/// </summary>
public enum LockState
{
    Locked,
    Unlocked
}
