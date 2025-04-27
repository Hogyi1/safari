using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemUiShop : MonoBehaviour
{

    public TMP_Text text;
    public Image itemImage;
    public Button ItemButton;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    public void SetItemData(Item item)
    {
      
        this.text.text = item.itemName;
        itemImage.sprite = item.imagePath;
        ItemButton.onClick.RemoveAllListeners(); 
        ItemButton.onClick.AddListener(() =>
        {
            ShopManager.Instance.ShowItemDetails(item);
        });
    }
    
    




}
