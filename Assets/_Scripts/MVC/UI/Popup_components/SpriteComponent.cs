using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UIKeys;

/// <summary>
/// A popup UI component responsible for displaying a sprite (e.g., icon).
/// Implements the IUIComponent interface for dynamic UI setup.
/// </summary>
public class SpriteComponent : MonoBehaviour, IUIComponent
{
    /// <summary>
    /// Reference to the UI Image component where the sprite will be rendered.
    /// </summary>
    [SerializeField] private Image image;

    /// <summary>
    /// The key used to retrieve the sprite from the popup data dictionary.
    /// </summary>
    [SerializeField] private UIKeys spriteKey = Sprite_icon;

    /// <summary>
    /// Attempts to assign a sprite from the provided data dictionary based on the spriteKey.
    /// </summary>
    /// <param name="data">Dictionary of popup UI data keyed by UIKeys enum.</param>
    public void TrySetup(Dictionary<UIKeys, object> data)
    {
        if (data.TryGetValue(spriteKey, out var icon))
        {
            image.sprite = (Sprite)icon;
        }
    }

    /// <summary>
    /// Called when the popup updates dynamically.
    /// Currently not used by this component.
    /// </summary>
    public void OnPopupUpdate() { }
}
