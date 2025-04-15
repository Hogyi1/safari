using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UIComponent;
public class SliderComponent : MonoBehaviour, IUIComponent
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI ProgressText;
    [SerializeField] private GameObject parent;
    private UIComponent maxKey = MaxValue_slider;
    private UIComponent capKey = Value_slider;

    private Func<float> getCurrentValue;
    private float maxValue;
    // Megadjuk a komponensnek a megadott értékekeket
    public void TrySetup(Dictionary<UIComponent, object> data)
    {
        if (data.TryGetValue(capKey, out var cap) && data.TryGetValue(maxKey, out var maxcap))
        {
            if (cap is Func<float> capFunc)
            {
                getCurrentValue = capFunc;
            }
            else
            {
                float fixedValue = Convert.ToSingle(cap);
                getCurrentValue = () => fixedValue;
            }
            maxValue = (int)maxcap;
            slider.maxValue = maxValue;
            parent.gameObject.SetActive(true);
        }
        else
        {
            parent.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (getCurrentValue != null)
        {
            float current = getCurrentValue();
            slider.value = current;
            ProgressText.text = $"{(int)current}/{(int)maxValue}";
        }
    }
}
