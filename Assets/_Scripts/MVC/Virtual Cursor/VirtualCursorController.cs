using UnityEngine;

/// <summary>
/// Controls the visibility and behavior of a virtual cursor UI in response
/// to UI mode toggles and input device changes.
/// </summary>
public class VirtualCursorController : MonoBehaviour
{
    /// <summary>
    /// Reference to the PlayerInputController used to re-enable player input when exiting UI mode.
    /// </summary>
    [SerializeField] private PlayerInputController playerInputController;

    /// <summary>
    /// GameObject representing the on-screen virtual cursor UI.
    /// </summary>
    [SerializeField] private GameObject virtualCursorUI;

    /// <summary>
    /// Subscribes to UI mode toggle and input device change events.
    /// </summary>
    private void OnEnable()
    {
        InputEventChannel.OnToggleUIMode += HandleToggleUIMode;

        InputDeviceDetector.OnDeviceChanged += HandleDeviceChange;
    }

    /// <summary>
    /// Unsubscribes from UI mode toggle and input device change events.
    /// </summary>
    private void OnDisable()
    {
        InputEventChannel.OnToggleUIMode -= HandleToggleUIMode;

        InputDeviceDetector.OnDeviceChanged -= HandleDeviceChange;
    }

    /// <summary>
    /// Shows or hides the virtual cursor UI based on whether UI mode is active.
    /// </summary>
    /// <param name="active">True to activate UI mode and show the virtual cursor; false to hide it.</param>
    private void HandleToggleUIMode(bool active)
    {
        virtualCursorUI.SetActive(active);
    }

    /// <summary>
    /// Ensures that when switching back to mouse/keyboard input,
    /// the virtual cursor UI is hidden and player input is restored.
    /// </summary>
    /// <param name="device">The newly active input device type.</param>
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
