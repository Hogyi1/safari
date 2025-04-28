using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryView : MonoBehaviour
{

    public GameObject itemPrefab; 
    public Transform inventoryContent; 
    public ItemDetailPanelInventory detailPanel;
    public static InventoryView Instance;


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

    public void GenerateInventoryUI(Dictionary<Item,int> items)
    {
        HideDetailPanel();
        ClearUP();

        foreach (Item item in items.Keys)
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

    public void HideDetailPanel() {
        detailPanel.gameObject.SetActive(false);
    }

}
