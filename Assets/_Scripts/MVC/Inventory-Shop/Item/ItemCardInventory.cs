using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI component representing an item entry in the player's inventory.
/// Displays item image, ownership count, and handles item selection.
/// </summary>
public class ItemCardInventory : MonoBehaviour
{
    /// <summary>
    /// Text field displaying the amount of the item the player owns.
    /// </summary>
    [SerializeField] private TMP_Text count;

    /// <summary>
    /// Image component showing the item's icon.
    /// </summary>
    [SerializeField] private Image itemImage;

    /// <summary>
    /// Button that, when clicked, shows details of the selected item.
    /// </summary>
    [SerializeField] private Button itemButton;

    /// <summary>
    /// Sets up the UI elements to represent a specific item from the inventory.
    /// Binds the click event to open the item's detail panel.
    /// </summary>
    /// <param name="item">The item to display in the UI element.</param>
    public void SetItemData(Item item)
    {
        this.count.text = "Owned: " + InventoryManager.Instance.GetItemCount(item).ToString();
        this.itemImage.sprite = item.Image;

        itemButton.onClick.RemoveAllListeners();
        itemButton.onClick.AddListener(() =>
        {
            InventoryManager.Instance.ShowItemDetails(item);
        });
    }
}
