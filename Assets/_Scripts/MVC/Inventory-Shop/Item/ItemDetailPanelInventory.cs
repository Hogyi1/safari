using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ItemDetailPanelInventory : MonoBehaviour
{
    public TMP_Text itemNameText;
    public TMP_Text Type;
    public TMP_Text itemCategoryText;
    public TMP_Text capacity;
    public TMP_Text inInventory;
    public TMP_Text sellingPrice;

    public void Show(Item item)
    {
        this.itemNameText.text = item.itemName;
        this.Type.text = "Type";
        this.itemCategoryText.text = Convert.ToString(item.category);
        this.capacity.text = "capacity";
        this.inInventory.text = Convert.ToString(Inventory.Instance.GetItemCount(item));
        this.sellingPrice.text = Convert.ToString(item.CalculateSellingPrice()) + " $";
        this.gameObject.SetActive(true);
    }
}
