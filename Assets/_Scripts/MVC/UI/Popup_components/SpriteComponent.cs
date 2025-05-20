using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UIKeys;

public class SpriteComponent : MonoBehaviour, IUIComponent
{
    [SerializeField] private Image image;
    [SerializeField] private UIKeys spriteKey = Sprite_icon;

    public void TrySetup(Dictionary<UIKeys, object> data)
    {
        if (data.TryGetValue(spriteKey, out var icon))
        {
            image.sprite = (Sprite)icon;
        }
    }
    public void OnPopupUpdate() { }
}
