using UnityEngine;
using UnityEngine.InputSystem;

public class ControlPanelSwitcher : MonoBehaviour
{
    public GameObject mouseKeyboardPanel;
    public GameObject xboxPanel;
    public GameObject playStationPanel;

    private enum GamepadType
    {
        Xbox,
        PlayStation,
        Unknown
    }

    private void OnEnable()
    {
        InputDeviceDetector.OnDeviceChanged += UpdatePanels;
        UpdatePanels(InputDeviceDetector.LastUsedDevice);
    }

    private void OnDisable()
    {
        InputDeviceDetector.OnDeviceChanged -= UpdatePanels;
    }

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
