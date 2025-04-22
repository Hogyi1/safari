using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class VirtualCursorController : MonoBehaviour
{
    [SerializeField] private PlayerInputController playerInputController;
    [SerializeField] private GameObject virtualCursorUI;

    private void OnEnable()
    {
        InputEventChannel.OnToggleUIMode += HandleToggleUIMode;

        InputDeviceDetector.OnDeviceChanged += HandleDeviceChange;
    }

    private void OnDisable()
    {
        InputEventChannel.OnToggleUIMode -= HandleToggleUIMode;

        InputDeviceDetector.OnDeviceChanged -= HandleDeviceChange;
    }

    private void HandleToggleUIMode(bool active)
    {
        virtualCursorUI.SetActive(active);
    }

    private void HandleDeviceChange(InputDeviceDetector.InputDeviceType device)
    {
        if (device == InputDeviceDetector.InputDeviceType.MouseKeyboard)
        {
            // Force exit UI mode if active
            if (virtualCursorUI.activeSelf)
            {
                virtualCursorUI.SetActive(false);

                // Re-enable player input
                playerInputController.SetUIMode(false);
            }
        }
    }
}
