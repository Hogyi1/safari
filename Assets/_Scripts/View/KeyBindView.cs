using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class KeyBindView : MonoBehaviour
{
    public TextMeshProUGUI actionNameText;
    public Button rebindButton;
    public Image keyIcon;
    public Sprite keyboardIcon, xboxIcon, playstationIcon, switchIcon;

    private string actionName;
    private KeyBindController controller;

    public void Initialize(string actionName, string bindingPath, KeyBindController controller)
    {
        this.actionName = actionName;
        this.controller = controller;
        actionNameText.text = actionName;
        UpdateKeyDisplay(bindingPath);

        rebindButton.onClick.AddListener(() => controller.StartRebind(actionName));
    }

    public void UpdateKeyDisplay(string bindingPath)
    {
        rebindButton.GetComponentInChildren<TextMeshProUGUI>().text = InputControlPath.ToHumanReadableString(bindingPath, InputControlPath.HumanReadableStringOptions.OmitDevice);
    }

    public void UpdateIcon(InputDevice device)
    {
        if (device is Keyboard)
        {
            keyIcon.sprite = keyboardIcon; // Assign the keyboard icon
        }
        else if (device is Gamepad gamepad)
        {
            // Check the manufacturer of the Gamepad
            string manufacturer = gamepad.device.description.manufacturer.ToLower();

            if (manufacturer.Contains("microsoft"))
            {
                keyIcon.sprite = xboxIcon; // Xbox controller
            }
            else if (manufacturer.Contains("sony"))
            {
                keyIcon.sprite = playstationIcon; // PlayStation controller
            }
            else if (manufacturer.Contains("nintendo"))
            {
                keyIcon.sprite = switchIcon; // Nintendo Switch controller
            }
            else
            {
                keyIcon.sprite = xboxIcon; // Default icon if manufacturer is unknown
            }
        }
    }

}
