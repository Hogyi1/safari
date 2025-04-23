using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ItemUiInventory : MonoBehaviour
{

    public TMP_Text Count;
    public Image itemImage;
    public Button ItemButton;

    private Item itemData;
    private InventoryManager inventoryManager;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventoryManager = InventoryManager.Instance; 
    }

    public void SetItemData(Item item)
    {
        itemData = item;
        this.Count.text = Convert.ToString(item.Count);
        itemImage.sprite = item.ImagePath;
        ItemButton.onClick.RemoveAllListeners(); 
        ItemButton.onClick.AddListener(() =>
        {
            InventoryManager.Instance.ShowItemDetails(itemData);
        });
    }
    
    




}
