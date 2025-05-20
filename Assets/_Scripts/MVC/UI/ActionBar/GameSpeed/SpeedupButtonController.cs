using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controller component for the speed-up button, handling click events and updating the view based on the current time multiplier.
/// Requires a <see cref="Button"/> and <see cref="SpeedupButtonView"/> on the same GameObject.
/// </summary>
[RequireComponent(typeof(Button))]
public class SpeedupButtonController : MonoBehaviour
{
    /// <summary>
    /// Reference to the <see cref="Button"/> component for detecting clicks.
    /// </summary>
    private Button button;

    /// <summary>
    /// Reference to the <see cref="SpeedupButtonView"/> component for updating icons.
    /// </summary>
    private SpeedupButtonView view;

    /// <summary>
    /// Initializes references to required components and subscribes to the button's click event.
    /// </summary>
    private void Awake()
    {
        button = GetComponent<Button>();
        view = GetComponent<SpeedupButtonView>();
        button.onClick.AddListener(OnClick);
    }

    /// <summary>
    /// Handles the button click by speeding up time via <see cref="TimeManager"/> and updating the view icons.
    /// </summary>
    private void OnClick()
    {
        float timeMult = TimeManager.Instance.SpeedUpTime();
        view.CycleImages(timeMult);
    }
}