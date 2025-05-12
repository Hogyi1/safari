using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Rendering.Universal;

public class ItemUiInventory : MonoBehaviour
{
    public TMP_Text Count;
    public Image itemImage;
    public Button ItemButton;
    
    public void SetItemData(Item item)
    {
        this.Count.text = "Owned: " + Convert.ToString(InventoryManager.Instance.inventory.GetItemCount(item));
        this.itemImage.sprite = item.imagePath;
        ItemButton.onClick.RemoveAllListeners(); 
        ItemButton.onClick.AddListener(() =>
        {
            InventoryManager.Instance.ShowItemDetails(item);
        });
    }
    
}
