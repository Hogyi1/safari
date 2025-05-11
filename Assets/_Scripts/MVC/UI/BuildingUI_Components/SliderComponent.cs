using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static StructureUIValues;

/// <summary>
/// Component displaying a progress slider and text
/// within a structure popup UI, handling dynamic values.
/// </summary>
public class SliderComponent : MonoBehaviour, IStructureUIComponent
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

    private StructureUIValues maxKey = MaxValue_slider;
    private StructureUIValues capKey = Value_slider;

    private Func<float> getCurrentValue;
    private float maxValue;

    /// <summary>
    /// Configures slider maximum value and data retrieval
    /// based on provided popup data dictionary.
    /// </summary>
    /// <param name="data">Dictionary mapping UI value keys to dynamic data.</param>
    public void TrySetup(Dictionary<StructureUIValues, object> data)
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

    /// <summary>
    /// Updates slider value and progress text each frame
    /// based on current value function.
    /// </summary>
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
