using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI panel that displays detailed information about a specific shop item,
/// including name, type, category, inventory amount, price, and special attributes.
/// Also enables or disables the buy button based on player currency.
/// </summary>
public class ItemDetailPanelShop : MonoBehaviour
{
    /// <summary>
    /// Text field for the item's display name.
    /// </summary>
    [SerializeField] private TMP_Text itemNameText;

    /// <summary>
    /// Text field for the item's type or class (e.g., Carnivore, Structure).
    /// </summary>
    [SerializeField] private TMP_Text typeInfo;

    /// <summary>
    /// Button used to purchase the item.
    /// Will be enabled or disabled depending on affordability.
    /// </summary>
    [SerializeField] private Button buyButton;

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
    /// Label for the second additional detail (e.g., "Capacity", "Lifespan").
    /// </summary>
    [SerializeField] private TMP_Text otherTitle2;

    /// <summary>
    /// Value for the second additional detail.
    /// </summary>
    [SerializeField] private TMP_Text otherInfo2;

    /// <summary>
    /// Text field showing how many units of this item the player currently owns.
    /// </summary>
    [SerializeField] private TMP_Text inInventoryInfo;

    /// <summary>
    /// Text field showing the calculated selling price of the item.
    /// </summary>
    [SerializeField] private TMP_Text sellingPriceInfo;

    /// <summary>
    /// Populates the shop item detail panel with the provided item's information.
    /// Also determines if the item is purchasable.
    /// </summary>
    /// <param name="item">The item whose details will be displayed.</param>
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
        sellingPriceInfo.text = item.CalculateSellingPrice().ToString() + " $";

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => ShopManager.Instance.PurchaseItem(item));

        buyButton.interactable = ShopManager.Instance.CanBuy(item);
        gameObject.SetActive(true);
    }
}
