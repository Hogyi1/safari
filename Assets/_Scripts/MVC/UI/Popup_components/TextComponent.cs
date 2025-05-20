using TMPro;
using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// A popup UI component that displays dynamic or static text.
/// Supports optional prefix and suffix wrapping, and can use functions for live updates.
/// </summary>
public class TextComponent : MonoBehaviour, IUIComponent
{
    /// <summary>
    /// Reference to the TextMeshProUGUI field to display the text.
    /// </summary>
    [SerializeField] private TextMeshProUGUI textField;

    /// <summary>
    /// The key used to retrieve the text value or function from the popup data dictionary.
    /// </summary>
    [SerializeField] private UIKeys textKey;

    [Header("Optional Wrapping")]
    /// <summary>
    /// Optional text to prepend before the main value.
    /// </summary>
    [SerializeField] private string prefix = "";

    /// <summary>
    /// Optional text to append after the main value.
    /// </summary>
    [SerializeField] private string suffix = "";

    /// <summary>
    /// Internal function delegate that returns the final text to display.
    /// Includes prefix and suffix wrapping.
    /// </summary>
    private Func<string> getText;

    /// <summary>
    /// Attempts to set up the text component based on the provided popup data.
    /// Supports both static strings and dynamic Func&lt;string&gt; entries.
    /// </summary>
    /// <param name="data">Popup data dictionary keyed by UIKeys.</param>
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

    /// <summary>
    /// Called on each popup update to refresh the displayed text.
    /// </summary>
    public void OnPopupUpdate()
    {
        if (getText != null)
            textField.text = getText();
    }
}

