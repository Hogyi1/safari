using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static StructureUIValues;

/// <summary>
/// Component displaying structure ID, name, and remove button
/// within a structure popup UI, handling icon and removal.
/// </summary>
public class BaseComponent : MonoBehaviour, IStructureUIComponent
{
    /// <summary>
    /// Image component showing the structure's icon.
    /// </summary>
    [SerializeField] private Image Iconimage;

    /// <summary>
    /// Button used to remove the structure.
    /// </summary>
    [SerializeField] private Button remove;

    /// <summary>
    /// Text component displaying the structure's name.
    /// </summary>
    [SerializeField] private TextMeshProUGUI Nametext;
    private StructureUIValues idKey = ID;
    private StructureUIValues nameKey = Name_text;
    private StructureUIValues iconKey = Sprite_icon;

    /// <summary>
    /// Configures icon sprite, name text, and remove button
    /// based on provided popup data dictionary.
    /// </summary>
    /// <param name="data">Dictionary mapping UI value keys to dynamic data.</param>
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
