using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI panel that displays detailed information about a specific inventory item.
/// Shows name, type, category, quantity in inventory, additional attributes, and selling price.
/// </summary>
public class ItemDetailPanelInventory : MonoBehaviour
{
    /// <summary>
    /// Text field for the item's display name.
    /// </summary>
    [SerializeField] private TMP_Text itemNameText;

    /// <summary>
    /// Text field for the item's type (e.g., Carnivore, Structure, Vehicle).
    /// </summary>
    [SerializeField] private TMP_Text typeInfo;

    /// <summary>
    /// Text field for the item's category (e.g., Animal, Vegetation).
    /// </summary>
    [SerializeField] private TMP_Text categoryInfo;

    /// <summary>
    /// Label for the first additional detail (e.g., "Diet").
    /// </summary>
    [SerializeField] private TMP_Text otherTitle;

    /// <summary>
    /// Value for the first additional detail (e.g., "Herbivore").
    /// </summary>
    [SerializeField] private TMP_Text otherInfo;

    /// <summary>
    /// Label for the second additional detail (e.g., "Lifespan").
    /// </summary>
    [SerializeField] private TMP_Text otherTitle2;

    /// <summary>
    /// Value for the second additional detail (e.g., "40").
    /// </summary>
    [SerializeField] private TMP_Text otherInfo2;

    /// <summary>
    /// Text field showing how many units of this item are currently owned.
    /// </summary>
    [SerializeField] private TMP_Text inInventoryInfo;

    /// <summary>
    /// Text field showing the item's calculated selling price.
    /// </summary>
    [SerializeField] private TMP_Text sellingPriceInfo;

    /// <summary>
    /// Button field to sell the item
    /// </summary>
    [SerializeField] private Button sellButton;

    /// <summary>
    /// Button field to place item
    /// </summary>
    [SerializeField] private Button placeButton;

    /// <summary>
    /// Populates the panel with detailed information about the given item and activates it.
    /// </summary>
    /// <param name="item">The item whose details will be shown in the panel.</param>
    public void Show(Item item)
    {
        itemNameText.text = item.ItemName;
        typeInfo.text = item.GetDisplayType();
        categoryInfo.text = item.Category.ToString();

        OtherDetail detail = item.GetOtherDetail();
        otherTitle.text = detail.Title;
        otherInfo.text = detail.Info;
        otherTitle2.text = detail.Title2;
        otherInfo2.text = detail.Info2;

        inInventoryInfo.text = InventoryManager.Instance.GetItemCount(item).ToString();
        sellingPriceInfo.text = "$" + item.CalculateSellingPrice().ToString() + ",00";


        sellButton.onClick.RemoveAllListeners();
        placeButton.onClick.RemoveAllListeners();

        sellButton.onClick.AddListener(() => InventoryManager.Instance.SellItem(item));

        placeButton.onClick.AddListener(() => InventoryManager.Instance.StartPlacingItem(item));
        placeButton.gameObject.SetActive(item.Placeable);

        gameObject.SetActive(true);
    }
}
