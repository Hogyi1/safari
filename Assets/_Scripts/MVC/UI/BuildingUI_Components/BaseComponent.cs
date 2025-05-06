using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static StructureUIValues;

public class BaseComponent : MonoBehaviour, IStructureUIComponent
{
    [SerializeField] private Image Iconimage;
    [SerializeField] private Button remove;
    [SerializeField] private TextMeshProUGUI Nametext;
    private StructureUIValues idKey = ID;
    private StructureUIValues nameKey = Name_text;
    private StructureUIValues iconKey = Sprite_icon;

    public void TrySetup(Dictionary<StructureUIValues, object> data)
    {
        if (data.TryGetValue(nameKey, out var name))
        {
            Nametext.text = name.ToString();
        }

        if (data.TryGetValue(idKey, out var id))
        {
            remove.onClick.RemoveAllListeners();
            remove.onClick.AddListener(() => { StructureManager.Instance.RemoveStructure((int)id); PopupManager.Instance.HidePopup(); });
        }

        if (data.TryGetValue(iconKey, out var icon))
        {
            Iconimage.sprite = (Sprite)icon;
        }
    }

}
