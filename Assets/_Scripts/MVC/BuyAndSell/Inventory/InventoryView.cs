using System.Collections.Generic;
using UnityEngine;

public class InventoryView : MonoBehaviour
{

    public GameObject itemPrefab; // Az item UI prefab
    public Transform inventoryContent; // Az a UI konténer, ahová az itemeket generáljuk
    public ItemDetailPanelInventory detailPanel;
    public InventoryView Instance;


    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ClearUP()
    {

        foreach (Transform child in inventoryContent)
        {
            Destroy(child.gameObject);
        }

    }


    public void UpdateInventoryUI(List<Item> items)
    {
        ClearUP();

        foreach (Item item in items)
        {
            GameObject newItem = Instantiate(itemPrefab, inventoryContent);
            ItemUiInventory itemUI = newItem.GetComponent<ItemUiInventory>();
            if (itemUI != null)
            {
                itemUI.SetItemData(item);
            }
        }
    }

    public void DetailPanelUpdate(Item item) {

        detailPanel.Show(item);

    }








}
