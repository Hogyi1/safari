using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemUiShop : MonoBehaviour
{

    public TMP_Text text;
    public Image itemImage;
    public Button ItemButton;

    private Item itemData;
    private ShopManager shopManager;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        shopManager = ShopManager.Instance; 
    }

    public void SetItemData(Item item)
    {
        itemData = item;
        this.text.text = item.Name;
        itemImage.sprite = item.ImagePath;
        ItemButton.onClick.RemoveAllListeners(); 
        ItemButton.onClick.AddListener(() =>
        {
            ShopManager.Instance.ShowItemDetails(itemData);
        });
    }
    
    




}
