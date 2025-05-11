using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemUiService : MonoBehaviour
{


    public TMP_Text text;
    public Image itemImage;
    public Button ItemButton;

    public void SetItemData(Item item)
    {
        this.text.text = item.itemName;
        itemImage.sprite = item.imagePath;
        ItemButton.onClick.RemoveAllListeners();
        ItemButton.onClick.AddListener(() =>
        {
            ServiceManager.Instance.ShowItemDetails(item);
        });
    }

}
