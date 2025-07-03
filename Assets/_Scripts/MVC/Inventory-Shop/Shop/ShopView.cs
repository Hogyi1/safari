using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the visual representation of the shop, including item generation,
/// UI cleanup, and displaying item detail panels.
/// </summary>
public class ShopView : MonoBehaviour
{
    /// <summary>
    /// Prefab used to represent each shop item in the UI.
    /// </summary>
    [SerializeField] private GameObject itemPrefab;

    /// <summary>
    /// Parent transform that holds all instantiated shop item UI elements.
    /// </summary>
    [SerializeField] private Transform shopContent;

    /// <summary>
    /// UI panel used to display detailed information about a selected shop item.
    /// </summary>
    [SerializeField] private ItemDetailPanelShop detailPanel;

    /// <summary>
    /// Removes all existing shop item UI elements from the content area.
    /// Called before regenerating the item list.
    /// </summary>
    public void CleanUp()
    {
        foreach (Transform child in shopContent)
        {
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// Generates the list of shop item UI cards based on the provided item list.
    /// Also clears the previous content and hides the detail panel.
    /// </summary>
    /// <param name="items">The list of items to display in the shop UI.</param>
    public void GenerateShopItems(List<Item> items)
    {
        HideDetailPanel();
        CleanUp();

        foreach (Item item in items)
        {
            GameObject newItem = Instantiate(itemPrefab, shopContent);
            ItemCardShop itemUI = newItem.GetComponent<ItemCardShop>();
            if (itemUI != null)
            {
                itemUI.SetItemData(item);
            }
        }
    }

    /// <summary>
    /// Displays the detail panel with information for the selected item.
    /// </summary>
    /// <param name="item">The item to show details for.</param>
    public void ShowItemDetails(Item item) => detailPanel.Show(item);

    /// <summary>
    /// Hides the item detail panel.
    /// </summary>
    public void HideDetailPanel() => detailPanel.gameObject.SetActive(false);
}
