using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ShopView : MonoBehaviour
{


    public GameObject itemPrefab; // Az item UI prefab
    public Transform shopContent; // Az a UI konténer, ahová az itemeket generáljuk
    public ItemDetailPanelShop detailPanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void ClearUP()
    {

        foreach (Transform child in shopContent)
        {
            Destroy(child.gameObject);
        }

    }


    public void GenerateShopItems(List<Item> items)
    {
        ClearUP();
        foreach (Item item in items)
        {
            GameObject newItem = Instantiate(itemPrefab, shopContent);
            ItemUiShop itemUI = newItem.GetComponent<ItemUiShop>();
            if (itemUI != null)
            {
                itemUI.SetItemData(item);
            }
        }
    }

    public void ShowItemDetails(Item item)
    {
        detailPanel.Show(item);
    }




}
