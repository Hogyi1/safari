using UnityEngine;
using UnityEngine.InputSystem.UI;

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
        input.Global.OnClick.canceled += _ => InputEventChannel.RaiseClick();

        input.CameraControls.Move.performed += ctx => InputEventChannel.RaiseCameraMove(ctx.ReadValue<Vector2>());
        input.CameraControls.Rotate.performed += ctx => InputEventChannel.RaiseCameraRotate(ctx.ReadValue<Vector2>());
        input.CameraControls.Height.performed += ctx => InputEventChannel.RaiseCameraHeight(ctx.ReadValue<float>());
        input.CameraControls.ZoomScroll.performed += ctx => InputEventChannel.RaiseZoom(ctx.ReadValue<float>());
        input.CameraControls.ZoomTriggers.performed += ctx => InputEventChannel.RaiseZoom(ctx.ReadValue<float>());

        input.UI.Cancel.performed += _ => HandleCancel();
        input.UI.TogglePause.performed += _ => HandleExplicitPause();

        InputEventChannel.OnPauseToggled += SyncPauseState;
    }

    /// <summary>
    /// Disables the input action asset when this component is disabled.
    /// </summary>
    private void OnDisable()
    {
        input.UI.Cancel.performed -= _ => HandleCancel();
        input.UI.TogglePause.performed -= _ => HandleExplicitPause();

        InputEventChannel.OnPauseToggled -= SyncPauseState;

        input.Disable();
    }

    /// <summary>
    /// Processes the Cancel input: 
    /// if the pause menu is the topmost UI, it un-pauses the game; 
    /// otherwise, it closes the top UI panel if one is open, 
    /// or opens the pause menu if none are.
    /// </summary>
    private bool isPaused = false;

    /// <summary>
    /// Always keep our internal pause flag in line with whatever just happened.
    /// </summary>
    private void SyncPauseState(bool paused)
    {
        isPaused = paused;
    }

    /// <summary>
    /// Processes the Cancel input: 
    /// if the pause menu is currently on top of the UI stack, it toggles the game's pause state; 
    /// otherwise, it closes the topmost UI panel if one is open,
    /// or opens the pause menu if no panels are open.
    /// </summary>
    private void HandleCancel()
    {
        var top = UIStackService.Peek();

        if (top != null
            && top == PauseController.Instance.PauseMenuUI
            && InputManager.Instance.State == InputState.NormalMode)
        {
            TogglePause();
            return;
        }

        if (InputManager.Instance.State != InputState.NormalMode)
        {
            InputManager.Instance.SetState(InputState.NormalMode);
            return;
        }

        if (UIStackService.IsUIOpen())
        {
            UIStackService.Pop();
        }
        else
        {
            TogglePause();
        }
    }

    /// <summary>
    /// Always toggles pause; mapped only to gamepad Start/Menu.
    /// </summary>
    private void HandleExplicitPause()
    {
        if (InputDeviceDetector.LastUsedDevice == InputDeviceDetector.InputDeviceType.Gamepad)
            TogglePause();
    }

    /// <summary>
    /// Flip pause state and broadcast.
    /// </summary>
    private void TogglePause()
    {
        isPaused = !isPaused;
        InputEventChannel.RaisePauseToggled(isPaused);
    }
}
