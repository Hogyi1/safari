using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI component representing an item entry in the game's shop.
/// Displays item image, ownership count, and handles item selection.
/// </summary>
public class ItemCardShop : MonoBehaviour
{
    /// <summary>
    /// Text field displaying the amount of the item the player owns.
    /// </summary>
    [SerializeField] private TMP_Text itemName;

    /// <summary>
    /// Image component showing the item's icon.
    /// </summary>
    [SerializeField] private Image itemImage;

    /// <summary>
    /// Button that, when clicked, shows details of the selected item.
    /// </summary>
    [SerializeField] private Button itemButton;

    /// <summary>
    /// Sets up the UI elements to represent a specific item from the shop.
    /// Binds the click event to open the item's detail panel.
    /// </summary>
    /// <param name="item">The item to display in the UI element.</param>
    public void SetItemData(Item item)
    {
        this.itemName.text = "$" + item.GetPrice().ToString() + ",00";
        this.itemImage.sprite = item.Image;

        itemButton.onClick.RemoveAllListeners();
        itemButton.onClick.AddListener(() =>
        {
            ShopManager.Instance.ShowItemDetails(item);
        });
    }
}
