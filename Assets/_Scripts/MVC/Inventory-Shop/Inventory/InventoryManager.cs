using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public Inventory inventory;
    public InventoryView inventoryView;
    public static InventoryManager Instance;
    public EconomyManager economyManager;

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        if (inventory == null)
        {
            inventory = new Inventory();

        }
        inventoryView.GenerateInventoryUI(inventory.items);
    }

    void OnEnable()
    {
        inventoryView.GenerateInventoryUI(inventory.items);
    }
    void Start()
    {
        if (inventory == null){
            inventory = new Inventory();

        }
        inventoryView.GenerateInventoryUI(inventory.items);
    }


    public void FilterAll()
    {
       inventoryView.GenerateInventoryUI(inventory.items);
    }

    public void FilterCategory(int categoryIndex)
    {
        inventoryView.ClearUP();
        Category category = (Category)categoryIndex;

        // Csak a kiválasztott kategóriát rendereljük
        Dictionary<Item, int> filtered = new Dictionary<Item, int>();

        foreach (KeyValuePair<Item, int> entry in inventory.items)
        {
            if (entry.Key.category == category)
            {
                filtered.Add(entry.Key, entry.Value);
            }
        }
    
        inventoryView.GenerateInventoryUI(filtered);
    }
    public void StartPlacing()
    {
        PlacementManager.Instance.StartPlacingItem(inventory.currentItem.buildingId);
        PlacementManager.Instance.OnPlace += PlaceItem;
        inventoryView.gameObject.SetActive(false);
    }
    public void PlaceItem()
    {
        inventory.RemoveItem(inventory.currentItem);
        inventoryView.GenerateInventoryUI(inventory.items);

        if (inventory.HasItem(inventory.currentItem))
        {
            PlacementManager.Instance.StopPlacement();
        }
        inventoryView.gameObject.SetActive(true);
    }

    public void Sell() {

        if (inventory.CanSellItem(inventory.currentItem))
        {
            inventory.DecreaseItemCountOrRemove(inventory.currentItem);
            economyManager.AddMoney(inventory.currentItem.CalculateSellingPrice());
            inventoryView.GenerateInventoryUI(inventory.items);
            inventoryView.DetailPanelUpdate(inventory.currentItem);
            if (inventory.GetItemCount(inventory.currentItem) == 0)
            {
                inventoryView.HideDetailPanel();
            }
        }
        else
        {
            Debug.Log("Sellerror");
        }
    }

    public void ShowItemDetails(Item item)
    {
        inventoryView.DetailPanelUpdate(item);
        inventory.currentItem = item;
    }

    //amikor felveszi az adott objectet akkor a pickup miatt berakja az inventoryba hogy lehessen használni
    public void PickedUpItem(Item Item) { 
        inventory.AddItem(Item);
    }

 
}
