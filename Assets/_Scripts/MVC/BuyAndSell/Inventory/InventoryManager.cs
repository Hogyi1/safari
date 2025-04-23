using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public Inventory inventory;
    public InventoryView inventoryView;

    public static InventoryManager Instance;
   
   
    public EconomyManager economyManager;


  
    void Start()
    {
        inventoryView.UpdateInventoryUI(inventory.items);
    }


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

    private void OnEnable()
    {
        inventoryView.UpdateInventoryUI(inventory.items);
    }

    
    public void FilterAll()
    {
       inventoryView.UpdateInventoryUI(inventory.items);
    }

    public void FilterCategory(int categoryIndex)
    {
        inventoryView.ClearUP();
        List<Item> filtered = new();
        //intet convertál categoryvá
        Category category = (Category)categoryIndex;



        // Csak a kiválasztott kategóriát rendereljük
        foreach (Item item in inventory.items)
        {
            if (item.Category == category)
            {
               filtered.Add(item);
            }
        }


        inventoryView.UpdateInventoryUI(filtered);
    }

    public void PlaceItem() {


        //ide kell még a placingmanaggerbol hogy le lett e placelve és csak akkor hivni
        //az inventoryt el kell tunteni
        if (true)
        {
            inventory.PalaceItem(inventory.currentItem);
            inventoryView.UpdateInventoryUI(inventory.items);
        }
    }

    public void Sell() {

        if (inventory.CanSellItem(inventory.currentItem))
        {
            inventory.DecreaseItemCountOrRemove(inventory.currentItem);
            economyManager.AddMoney(inventory.currentItem.CalculateSellingPrice());
            inventoryView.UpdateInventoryUI(inventory.items);
            inventoryView.DetailPanelUpdate(inventory.currentItem);
        }
        else
        {
            Debug.Log("sell error");
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
