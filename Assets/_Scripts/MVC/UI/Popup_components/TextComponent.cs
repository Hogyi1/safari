using TMPro;
using UnityEngine;
using System;
using System.Collections.Generic;

public class TextComponent : MonoBehaviour, IUIComponent
{
    [SerializeField] private TextMeshProUGUI textField;
    [SerializeField] private UIKeys textKey;

    [Header("Optional Wrapping")]
    [SerializeField] private string prefix = "";
    [SerializeField] private string suffix = "";

    private Func<string> getText;

    public void TrySetup(Dictionary<UIKeys, object> data)
    {
        if (data.TryGetValue(textKey, out var val))
        {
            Func<string> baseGetter;
            if (val is Func<string> strFunc)
                baseGetter = strFunc;
            else
                baseGetter = () => val.ToString();

            getText = () => prefix + baseGetter() + suffix;

            textField.gameObject.SetActive(true);
        }
        else
        {
            textField.gameObject.SetActive(false);
            getText = null;
        }
    }

    public void OnPopupUpdate()
    {
        if (getText != null)
            textField.text = getText();
    }
}
