using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

/// <summary>
/// Manages shop logic including item filtering, purchasing, and UI updates.
/// Uses a singleton pattern to maintain a single global instance.
/// </summary>
public class ShopManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the ShopManager.
    /// </summary>
    public static ShopManager Instance;

    /// <summary>
    /// Reference to the ShopView responsible for rendering the shop UI.
    /// </summary>
    [SerializeField] private ShopView shopView;

    /// <summary>
    /// All items available in the shop.
    /// </summary>
    private List<Item> items;

    /// <summary>
    /// Initializes the singleton instance and ensures only one ShopManager exists.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this);
    }

    /// <summary>
    /// Loads all items from the ItemManager and generates the shop UI.
    /// </summary>
    private void Start()
    {
        items = ItemManager.Instance.GetItems();
        shopView.GenerateShopItems(items);
    }

   /// <summary>
    /// Attempts to purchase a given item. Deducts money, adds to inventory, or triggers spawn depending on category.
    /// </summary>
    /// <param name="item">The item to purchase.</param>
    public void PurchaseItem(Item item)
    {
        if (item.IsUnityNull()) return;

        int ID = item.GetPlacementID();

        switch (item.Category)
        {
            case Category.Service:
                // Handle service logic here if needed
                break;

            case Category.Vehicle:
                VehicleManager.Instance.SpawnVehicle(ID);
                break;

            case Category.Structure:
            case Category.Vegetation:
            case Category.Animal:
            default:
                InventoryManager.Instance.AddItem(item);
                EconomyManager.Instance.RemoveMoney(item.GetPrice());
                ShowItemDetails(item);
                break;
        }
    }


    /// <summary>
    /// Displays all available items in the shop without filtering.
    /// </summary>
    public void FilterAll() => shopView.GenerateShopItems(items);

    /// <summary>
    /// Filters items by the given category index and updates the shop UI.
    /// </summary>
    /// <param name="index">Index of the category enum.</param>
    public void FilterCategory(int index)
    {
        Category category = (Category)index;
        List<Item> filtered = items.FindAll(item => item.Category == category);
        shopView.GenerateShopItems(filtered);
    }

    /// <summary>
    /// Displays detailed information about the given item in the shop UI.
    /// </summary>
    /// <param name="item">The item to show details for.</param>
    public void ShowItemDetails(Item item) => shopView.ShowItemDetails(item);

    /// <summary>
    /// Determines whether the player has enough money and available capacity to buy the item.
    /// </summary>
    /// <param name="item">The item to check.</param>
    /// <returns>True if the item is purchasable, false otherwise.</returns>
    public bool CanBuy(Item item)
    {
        bool enoughMoney = EconomyManager.Instance.HasEnoughMoney(item.GetPrice());
        IBuyableManager manager = GetManagerByItem(item);
        bool hasSpace = manager != null && manager.CanBuy();
        bool unlocked = item.LockState == LockState.Unlocked;

        return enoughMoney && hasSpace && unlocked;
    }

    /// <summary>
    /// Returns the appropriate manager responsible for handling item purchases based on item category.
    /// </summary>
    /// <param name="item">The item to resolve the manager for.</param>
    /// <returns>An IBuyableManager implementation, or null if not applicable.</returns>
    private IBuyableManager GetManagerByItem(Item item)
    {
        switch (item.Category)
        {
            case Category.Vehicle:
                return VehicleManager.Instance;

            case Category.Structure:
            case Category.Vegetation:
                return StructureManager.Instance;

            case Category.Animal:
                return AnimalManager.Instance;

            case Category.Service:
            default:
                return null;
        }
    }
}

/// <summary>
/// Interface for managers that handle buyable item categories (e.g., vehicles, buildings, animals).
/// Used to check capacity or slot availability before purchase.
/// </summary>
public interface IBuyableManager
{
    /// <summary>
    /// Determines whether the manager has space to accept a new item.
    /// </summary>
    /// <returns>True if a new item can be bought/placed; false otherwise.</returns>
    bool CanBuy();
}
