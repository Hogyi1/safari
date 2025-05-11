using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServiceView : MonoBehaviour
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


    public void GenerateServiceItems(HashSet<Item> items)
    {
        HideDetailPanel();
        ClearUP();
        foreach (Item item in items)
        {
            GameObject newItem = Instantiate(itemPrefab, shopContent);
            ItemUiService itemUI = newItem.GetComponent<ItemUiService>();
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

    public void HideDetailPanel()
    {
        detailPanel.gameObject.SetActive(false);
    }

}


