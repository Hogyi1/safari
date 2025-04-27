using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopView : MonoBehaviour
{
    public Button buybutton;
    public GameObject itemPrefab; 
    public Transform shopContent; 
    public ItemDetailPanelShop detailPanel;

    public void ClearUP()
    {

        foreach (Transform child in shopContent)
        {
            Destroy(child.gameObject);
        }

    }


    public void GenerateShopItems(HashSet<Item> items)
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
