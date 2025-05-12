using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the visual representation of the inventory UI.
/// Responsible for generating item cards, clearing content, and showing item details.
/// </summary>
public class InventoryView : MonoBehaviour
{
    /// <summary>
    /// Prefab used to visually represent each inventory item in the UI.
    /// </summary>
    [SerializeField] private GameObject itemPrefab;

    /// <summary>
    /// Parent container that holds all instantiated inventory item UI elements.
    /// </summary>
    [SerializeField] private Transform inventoryContent;

    /// <summary>
    /// UI panel used to display detailed information about the selected inventory item.
    /// </summary>
    [SerializeField] private ItemDetailPanelInventory detailPanel;

    /// <summary>
    /// Removes all existing inventory item UI elements from the content area.
    /// Called before regenerating the inventory list.
    /// </summary>
    public void CleanUp()
    {
        foreach (Transform child in inventoryContent)
        {
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// Generates the inventory UI based on the provided item dictionary.
    /// Instantiates item cards and hides the detail panel.
    /// </summary>
    /// <param name="items">A dictionary of items and their quantities in the inventory.</param>
    public void GenerateInventoryUI(Dictionary<Item, int> items)
    {
        HideDetailPanel();
        CleanUp();

        foreach (Item item in items.Keys)
        {
            GameObject newItem = Instantiate(itemPrefab, inventoryContent);
            ItemCardInventory itemUI = newItem.GetComponent<ItemCardInventory>();
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
