using UnityEngine;

/// <summary>
/// Dispatches input actions from the generated InputSystem_Actions asset
/// to the centralized InputEventChannel for decoupled event handling.
/// </summary>
public class InputDispatcherService : MonoBehaviour
{
    /// <summary>
    /// The input action asset containing action maps.
    /// </summary>
    private InputSystem_Actions input;

    /// <summary>
    /// Initializes and enables the input action asset on Awake.
    /// </summary>
    private void Awake()
    {
        input = new InputSystem_Actions();
        input.Enable();
    }

    /// <summary>
    /// Subscribes to input action callbacks and raises corresponding events.
    /// </summary>
    private void OnEnable()
    {
        input.Player.Look.performed += ctx => InputEventChannel.RaiseLook(ctx.ReadValue<Vector2>());
        input.UI.RightClick.performed += _ => InputEventChannel.RaiseRightClick();

        input.CameraControls.Move.performed += ctx => InputEventChannel.RaiseCameraMove(ctx.ReadValue<Vector2>());
        input.CameraControls.Rotate.performed += ctx => InputEventChannel.RaiseCameraRotate(ctx.ReadValue<Vector2>());
        input.CameraControls.Height.performed += ctx => InputEventChannel.RaiseCameraHeight(ctx.ReadValue<float>());
        input.CameraControls.ZoomScroll.performed += ctx => InputEventChannel.RaiseZoom(ctx.ReadValue<float>());
        input.CameraControls.ZoomTriggers.performed += ctx => InputEventChannel.RaiseZoom(ctx.ReadValue<float>());

        input.UI.TogglePause.performed += _ => TogglePause();
    }

    /// <summary>
    /// Disables the input action asset when this component is disabled.
    /// </summary>
    private void OnDisable()
    {
        input.Disable();
    }

    /// <summary>
    /// Tracks the current paused state of the game.
    /// </summary>
    private bool isPaused = false;

    /// <summary>
    /// Toggles the paused state and raises the pause toggled event.
    /// </summary>
    private void TogglePause()
    {
        isPaused = !isPaused;
        InputEventChannel.RaisePauseToggled(isPaused);
    }
}
