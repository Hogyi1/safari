using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ItemDetailPanelShop : MonoBehaviour
{
    public TMP_Text itemNameText;
    public TMP_Text Type;
    public TMP_Text itemCategoryText;
    public TMP_Text capacity;
    public TMP_Text inInventory;
    public TMP_Text sellingPrice;
    public Button buyButton;
    

    public void Show(Item item)
    {
        this.itemNameText.text = item.itemName;
        this.Type.text = "asd";
        this.itemCategoryText.text = Convert.ToString(item.category);
        this.capacity.text = "";
        this.inInventory.text = Convert.ToString(InventoryManager.Instance.inventory.GetItemCount(item));
        this.sellingPrice.text = Convert.ToString(item.CalculateSellingPrice()) + " $";
        buyButton.enabled = EconomyManager.Instance.HasEnoughMoney(item.price);
        this.gameObject.SetActive(true);
    }
}   