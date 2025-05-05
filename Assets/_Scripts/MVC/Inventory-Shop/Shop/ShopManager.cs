using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{   
    public ShopView shopView;
    public static ShopManager Instance;
    public HashSet<Item> items;
    public Item currentItem;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        items =  ItemManager.Instance.getItems();
        shopView.GenerateShopItems(items);
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
        if (currentItem != null)
        {
                Inventory.Instance.AddItem(currentItem);
                EconomyManager.Instance.RemoveMoney(currentItem.price);
                ShowItemDetails(currentItem);
                Debug.Log("megvette");
        }
        
    }
   
    public void FilterAll()
    {
        shopView.GenerateShopItems(items); 
    }

    public void FilterCategory(int index)   
    {
        Category category = (Category)index;
        
        HashSet<Item> filtered = new HashSet<Item>();
        foreach (Item item in items)
        {
            if (item.category == category)
            {
                
                filtered.Add(item);
            }
        }
        shopView.GenerateShopItems(filtered);
    }

  
    public void ShowItemDetails(Item item) {
        shopView.ShowItemDetails(item);
        currentItem = item;
    }

}
