using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UIComponent;

public class BaseComponent : MonoBehaviour, IUIComponent
{
    [SerializeField] private Image Iconimage;
    [SerializeField] private Button remove;
    [SerializeField] private TextMeshProUGUI Nametext;
    private UIComponent idKey = ID;
    private UIComponent nameKey = Name_text;
    private UIComponent iconKey = Sprite_icon;

    public void TrySetup(Dictionary<UIComponent, object> data)
    {
        if (data.TryGetValue(idKey, out var id))
        {
            remove.onClick.RemoveAllListeners();
            remove.onClick.AddListener(() => { StructureManager.Instance.RemoveStructure((int)id); PopupManager.Instance.HidePopup(); });
        }

        if (data.TryGetValue(nameKey, out var name))
        {
            Nametext.text = name.ToString();
        }

        if (data.TryGetValue(iconKey, out var icon))
        {
            Iconimage.sprite = (Sprite)icon;
        }
    }

}
