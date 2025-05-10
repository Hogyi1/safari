using System;
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
    [SerializeField] private TextMeshProUGUI LevelText;
    private StructureUIValues idKey = ID;
    private StructureUIValues nameKey = Name_text;
    private StructureUIValues iconKey = Sprite_icon;
    private StructureUIValues levelKey = Level;

    private bool canRemove;
    private Func<int> getLevel;

    public void TrySetup(Dictionary<StructureUIValues, object> data)
    {
        canRemove = true;
        if (data.TryGetValue(nameKey, out var name))
        {
            Nametext.text = name.ToString();
        }

        if (data.TryGetValue(levelKey, out var lvlObj))
        {
            canRemove = false;
            LevelText.gameObject.SetActive(true);

            if (lvlObj is Func<int> lvlFunc)
            {
                getLevel = lvlFunc;
            }
            else
            {
                int fixedLvl = Convert.ToInt32(lvlObj);
                getLevel = () => fixedLvl;
            }
        }
        else
        {
            LevelText.gameObject.SetActive(false);
            getLevel = null;
        }

        if (data.TryGetValue(iconKey, out var icon))
        {
            Iconimage.sprite = (Sprite)icon;
        }

        if (data.TryGetValue(idKey, out var id) && canRemove)
        {
            remove.gameObject.SetActive(true);
            remove.onClick.RemoveAllListeners();
            remove.onClick.AddListener(() => { StructureManager.Instance.RemoveStructure((int)id); PopupManager.Instance.HidePopup(); });
        }
        else
        {
            remove.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (getLevel != null)
        {
            LevelText.text = $"Level {getLevel()}";
        }
    }
}
