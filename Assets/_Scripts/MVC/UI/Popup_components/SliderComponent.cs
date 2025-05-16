using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UIKeys;

/// <summary>
/// Component displaying a progress slider and text
/// within a structure popup UI, handling dynamic values.
/// </summary>
public class SliderComponent : MonoBehaviour, IPopupComponent
{
    /// <summary>
    /// Slider UI element showing current progress.
    /// </summary>
    [SerializeField] private Slider slider;

    /// <summary>
    /// Text component displaying current and max values.
    /// </summary>
    [SerializeField] private TextMeshProUGUI ProgressText;

    /// <summary>
    /// Parent GameObject enabling or disabling the slider UI.
    /// </summary>
    [SerializeField] private GameObject parent;

    [SerializeField] private UIKeys maxKey = MaxValue_slider;
    [SerializeField] private UIKeys capKey = Value_slider;
    [SerializeField] private UIKeys healthbar = Healthbar;
    [SerializeField] private UIKeys vehicles = UIKeys.Vehicle;

    [SerializeField] private Image fill;
    [SerializeField] private Color health;
    [SerializeField] private Color normal;
    [SerializeField] private Color vehicle;

    private Func<float> getCurrentValue;
    private Func<float> getMaxValue;

    /// <summary>
    /// Configures slider maximum value and data retrieval
    /// based on provided popup data dictionary.
    /// </summary>
    /// <param name="data">Dictionary mapping UI value keys to dynamic data.</param>
    public void TrySetup(Dictionary<UIKeys, object> data)
    {
        if (data.TryGetValue(healthbar, out var hp)) fill.color = health;
        else if (data.TryGetValue(vehicles, out var car)) fill.color = vehicle;
        else fill.color = normal;

        if (data.TryGetValue(capKey, out var cap) && data.TryGetValue(maxKey, out var maxcap))
        {
            if (cap is Func<float> capFunc) getCurrentValue = capFunc;
            else
            {
                float fixedValue = Convert.ToSingle(cap);
                getCurrentValue = () => fixedValue;
            }

            if (maxcap is Func<float> maxcapFunc) getMaxValue = maxcapFunc;
            else
            {
                float fixedMax = Convert.ToSingle(maxcap);
                getMaxValue = () => fixedMax;
            }

            parent.gameObject.SetActive(true);
        }
        else parent.gameObject.SetActive(false);
    }

    /// <summary>
    /// Updates slider value and progress text each frame
    /// based on current value function.
    /// </summary>
    public void OnPopupUpdate()
    {
        if (getCurrentValue != null && getMaxValue != null)
        {
            float current = getCurrentValue();
            float max = getMaxValue();

            slider.maxValue = max;
            slider.value = current;
            Debug.Log("Max: " + max + " Current: " + current);

            if (fill.color == health) ProgressText.text = "";
            else ProgressText.text = $"{(int)current}/{(int)max}";
        }
    }
}
