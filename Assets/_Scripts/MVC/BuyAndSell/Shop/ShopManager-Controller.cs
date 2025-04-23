using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{

    public ShopModell shopModell;
    public ShopView shopView;

    public static ShopManager Instance;
    public Inventory inventory;
    public EconomyManager economyManager;
    public InventoryManager inventoryManager;



   

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventoryManager = GetComponent<InventoryManager>();
        shopView.GenerateShopItems(shopModell.items);
    }

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public void PurchaseItem()
    {
        //mivel nincs más lehetõség vásárlásra ezért amikor rányom egy itemre csak akkor tudja megvenni
        if (shopModell.currentItem != null)
        {
            if (economyManager.HasEnoughMoney(shopModell.currentItem.Price))
            {
                inventory.AddItem(shopModell.currentItem);
                economyManager.RemoveMoney(shopModell.currentItem.Price);
                ShowItemDetails(shopModell.currentItem);
                Debug.Log("megvette");
            }
            else
            {
                Debug.Log("nincs pénzed");
            }
        }
        
    }

    public void FilterAll()
    {
        shopView.GenerateShopItems(shopModell.items); 
    }

    public void FilterCategory(int categoryIndex)   
    {
        //intet convertál categoryvá
        Category category = (Category)categoryIndex;
        List<Item> filtered = new();
        // Csak a kiválasztott kategóriát rendereljük
        foreach (Item item in shopModell.items)
        {
            if (item.Category == category)
            {
                filtered.Add(item);
                
            }
        }
        shopView.GenerateShopItems(filtered);
    }

    

   

    public void ShowItemDetails(Item item) {
        shopView.ShowItemDetails(item);
        shopModell.currentItem = item;
    }


}
