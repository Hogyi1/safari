using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles enabling and disabling player input and toggling UI mode.
/// </summary>
public class PlayerInputController : MonoBehaviour
{
    /// <summary>
    /// Generated input actions for handling global inputs.
    /// </summary>
    private InputSystem_Actions input;

    /// <summary>
    /// Reference to the PlayerInput component used to enable or disable player controls.
    /// </summary>
    [SerializeField] private PlayerInput playerInput;

    /// <summary>
    /// Tracks whether UI mode is currently active.
    /// </summary>
    private bool uiModeActive = false;

    /// <summary>
    /// Initializes input action asset and enables it.
    /// </summary>
    private void Awake()
    {
        input = new InputSystem_Actions();
        input.Enable();
    }

    /// <summary>
    /// Subscribes to the ToggleUI action when the component is enabled.
    /// </summary>
    private void OnEnable()
    {
        input.Global.ToggleUI.performed += ctx => ToggleUI();
    }

    /// <summary>
    /// Disables all input actions when the component is disabled.
    /// </summary>
    private void OnDisable()
    {
        input.Disable();
    }

    /// <summary>
    /// Toggles the UI mode on or off.
    /// </summary>
    private void ToggleUI()
    {
        SetUIMode(!uiModeActive);
    }

    /// <summary>
    /// Activates or deactivates UI mode, enabling or disabling player input accordingly.
    /// </summary>
    /// <param name="active">Whether UI mode should be active.</param>
    public void SetUIMode(bool active)
    {
        uiModeActive = active;
        playerInput.enabled = !active;

        InputEventChannel.RaiseToggleUIMode(active);
    }
}
