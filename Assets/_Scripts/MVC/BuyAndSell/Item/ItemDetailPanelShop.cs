using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ItemDetailPanelShop : MonoBehaviour
{


    public GameObject panelToShow;
    public TMP_Text itemNameText;
    public TMP_Text itemPriceText;
    public TMP_Text itemCategoryText;
    public TMP_Text capacity;
    public TMP_Text inInventory;
    public TMP_Text sellingPrice;


    public void Show(Item item)
    {
        itemNameText.text = item.Name;
        itemPriceText.text = $"{item.Price} Coins";
        itemCategoryText.text = Convert.ToString(item.Category);
        inInventory.text = Convert.ToString(item.Count);
        sellingPrice.text = Convert.ToString(item.CalculateSellingPrice());
        gameObject.SetActive(true);
    }

    public void ShowPanel()
    {
        panelToShow.SetActive(true);
    }

}
