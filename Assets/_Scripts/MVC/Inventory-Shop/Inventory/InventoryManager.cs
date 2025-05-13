using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

/// <summary>
/// Manages the player's inventory logic, including item placement, filtering, selling, and UI updates.
/// Acts as a bridge between the inventory data, UI, and placement systems.
/// </summary>
public class InventoryManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the InventoryManager.
    /// </summary>
    public static InventoryManager Instance;

    /// <summary>
    /// Reference to the inventory UI handler.
    /// </summary>
    [SerializeField] private InventoryView inventoryView;

    /// <summary>
    /// Internal inventory data structure.
    /// </summary>
    private Inventory inventory;

    /// <summary>
    /// Currently selected item for placement.
    /// </summary>
    private Item currentItem;

    /// <summary>
    /// Initializes the singleton instance.
    /// </summary>
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

    /// <summary>
    /// Initializes the inventory if needed and generates the UI.
    /// </summary>
    void Start()
    {
        if (inventory == null) inventory = new Inventory();
        inventoryView.GenerateInventoryUI(inventory.Items);
    }

    /// <summary>
    /// Displays the entire inventory in the UI without filtering.
    /// </summary>
    public void FilterAll() => inventoryView.GenerateInventoryUI(inventory.Items);

    /// <summary>
    /// Filters inventory items by category and updates the UI.
    /// </summary>
    /// <param name="categoryIndex">The integer index of the selected category enum.</param>
    public void FilterCategory(int categoryIndex)
    {
        inventoryView.CleanUp();
        Category category = (Category)categoryIndex;

        var filtered = inventory.Items
            .Where(entry => entry.Key.Category == category)
            .ToDictionary(entry => entry.Key, entry => entry.Value);

        inventoryView.GenerateInventoryUI(filtered);
    }

    /// <summary>
    /// Returns the appropriate placement manager for the given item.
    /// </summary>
    /// <param name="item">The item to get the manager for.</param>
    /// <returns>An instance of IPlaceableManager.</returns>
    private IPlaceableManager GetManager(Item item)
    {
        Category category = item.Category;
        switch (category)
        {
            case Category.Animal:
                return AnimalManager.Instance;
            case Category.Structure:
            case Category.Vegetation:
            default:
                return PlacementManager.Instance;
        }
    }

    /// <summary>
    /// Starts the placement process for the selected item.
    /// </summary>
    /// <param name="item">The item to place.</param>
    public void StartPlacingItem(Item item)
    {
        IPlaceableManager manager = GetManager(item);
        currentItem = item;

        manager.OnPlaced += PlaceItem;
        manager.OnStopped += StopPlacement;

        manager.StartPlacing(currentItem.GetPlacementID());
        inventoryView.gameObject.SetActive(false);
    }

    /// <summary>
    /// Placement stopped open inventory
    /// </summary>
    private void StopPlacement()
    {
        if (currentItem == null) return;
        IPlaceableManager manager = GetManager(currentItem);
        manager.OnStopped -= StopPlacement;
        manager.OnPlaced -= PlaceItem;

        inventoryView.GenerateInventoryUI(inventory.Items);
        inventoryView.gameObject.SetActive(true);
    }

    /// <summary>
    /// Callback method invoked when the current item has been placed.
    /// Updates the inventory and re-enables the UI if needed.
    /// </summary>
    public void PlaceItem()
    {
        inventory.RemoveItem(currentItem);
        inventoryView.GenerateInventoryUI(inventory.Items);

        IPlaceableManager manager = GetManager(currentItem);

        if (!inventory.HasItem(currentItem))
        {
            manager.StopPlacement();
            manager.OnPlaced -= PlaceItem;
            inventoryView.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Sells the given item if possible, adds money, and updates the UI.
    /// </summary>
    /// <param name="item">The item to sell.</param>
    public void SellItem(Item item)
    {
        if (!CanSellItem(item)) return;

        inventory.DecreaseItemCountOrRemove(item);
        EconomyManager.Instance.AddMoney(item.CalculateSellingPrice());
        inventoryView.GenerateInventoryUI(inventory.Items);

        if (inventory.HasItem(item))
        {
            inventoryView.ShowItemDetails(item);
        }
    }

    /// <summary>
    /// Adds an item to the inventory based on a placed object's ID.
    /// </summary>
    /// <param name="placedID">The placement ID of the object to match to an item.</param>
    public void PickedUpItem(int placedID)
    {
        List<Item> allItem = ItemManager.Instance.GetItems();
        Item selectedItem = allItem.FirstOrDefault(t => t.Placeable && t.GetPlacementID() == placedID && t.Category != Category.Animal);
        if (selectedItem == null) return;

        inventory.AddItem(selectedItem);
    }

    /// <summary>
    /// Displays item details in the UI.
    /// </summary>
    /// <param name="item">The item to show details for.</param>
    public void ShowItemDetails(Item item) => inventoryView.ShowItemDetails(item);

    /// <summary>
    /// Checks if the item can be sold.
    /// </summary>
    /// <param name="item">The item to check.</param>
    /// <returns>True if the item is sellable, false otherwise.</returns>
    public bool CanSellItem(Item item) => inventory.CanSellItem(item);

    /// <summary>
    /// Adds an item to the inventory.
    /// </summary>
    /// <param name="item">The item to add.</param>
    public void AddItem(Item item) => inventory.AddItem(item);

    /// <summary>
    /// Returns the amount of the given item in the inventory.
    /// </summary>
    /// <param name="item">The item to check.</param>
    /// <returns>The quantity of the item in inventory.</returns>
    public object GetItemCount(Item item) => inventory.GetItemCount(item);
}

/// <summary>
/// Interface for managers that handle placement of items (e.g., structures, animals).
/// Provides events for when placement is completed or cancelled.
/// </summary>
public interface IPlaceableManager
{
    /// <summary>
    /// Begins the placement process for a given object by its ID.
    /// </summary>
    /// <param name="ID">The ID of the placeable object.</param>
    void StartPlacing(int ID);

    /// <summary>
    /// Cancels or ends the placement process.
    /// </summary>
    void StopPlacement();

    /// <summary>
    /// Event triggered when the object has been successfully placed.
    /// </summary>
    event Action OnPlaced;

    /// <summary>
    /// Event triggered when the placement is stopped or cancelled.
    /// </summary>
    event Action OnStopped;
}

