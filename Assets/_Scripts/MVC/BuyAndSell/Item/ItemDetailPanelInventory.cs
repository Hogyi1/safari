using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ItemDetailPanelInventory : MonoBehaviour
{
    public TMP_Text itemNameText;
    public TMP_Text itemPriceText;
    public TMP_Text itemCategoryText;
    public TMP_Text capacity;
    public TMP_Text inInventory;
    public TMP_Text sellingPrice;
    
    public void Show(Item item)
    {
        itemNameText.text = item.itemName;
        itemPriceText.text = $"{item.price} Coins";
        itemCategoryText.text = Convert.ToString(item.category);
        inInventory.text = Convert.ToString(Inventory.Instance.GetItemCount(item));
        sellingPrice.text = Convert.ToString(item.CalculateSellingPrice());
        gameObject.SetActive(true);
    }
    

}
