using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Switches control panel UI based on the last used input device (mouse/keyboard, Xbox, PlayStation).
/// </summary>
public class ControlPanelSwitcher : MonoBehaviour
{
    /// <summary>UI panel displayed for mouse and keyboard input.</summary>
    public GameObject mouseKeyboardPanel;
    /// <summary>UI panel displayed for Xbox controller input.</summary>
    public GameObject xboxPanel;
    /// <summary>UI panel displayed for PlayStation controller input.</summary>
    public GameObject playStationPanel;

    /// <summary>Types of gamepad brands supported.</summary>
    private enum GamepadType
    {
        Xbox,
        PlayStation,
        Unknown
    }

    /// <summary>
    /// Subscribes to input device change events and initializes the control panels.
    /// </summary>
    private void OnEnable()
    {
        InputDeviceDetector.OnDeviceChanged += UpdatePanels;
        UpdatePanels(InputDeviceDetector.LastUsedDevice);
    }

    /// <summary>
    /// Unsubscribes from input device change events.
    /// </summary>
    private void OnDisable()
    {
        InputDeviceDetector.OnDeviceChanged -= UpdatePanels;
    }

    /// <summary>
    /// Activates the appropriate panel based on the current input device.
    /// </summary>
    /// <param name="deviceType">The type of the last used input device.</param>
    private void UpdatePanels(InputDeviceDetector.InputDeviceType deviceType)
    {
        mouseKeyboardPanel.SetActive(false);
        xboxPanel.SetActive(false);
        playStationPanel.SetActive(false);

        switch (deviceType)
        {
            case InputDeviceDetector.InputDeviceType.MouseKeyboard:
                mouseKeyboardPanel.SetActive(true);
                break;

            case InputDeviceDetector.InputDeviceType.Gamepad:
                var gamepad = Gamepad.current;
                var gamepadType = GetGamepadType(gamepad);

                if (gamepadType == GamepadType.PlayStation)
                    playStationPanel.SetActive(true);
                else
                    xboxPanel.SetActive(true);
                break;
        }
    }

    /// <summary>
    /// Determines the brand type of the specified gamepad device.
    /// </summary>
    /// <param name="gamepad">The gamepad device to evaluate.</param>
    /// <returns>The detected GamepadType (Xbox, PlayStation, or Unknown).</returns>
    private GamepadType GetGamepadType(Gamepad gamepad)
    {
        if (gamepad == null) return GamepadType.Unknown;

        var product = gamepad.description.product?.ToLower();
        if (string.IsNullOrEmpty(product)) return GamepadType.Unknown;

        if (product.Contains("playstation") || product.Contains("dualshock") || product.Contains("dualsense"))
            return GamepadType.PlayStation;

        if (product.Contains("xbox"))
            return GamepadType.Xbox;

        return GamepadType.Unknown;
    }
}
