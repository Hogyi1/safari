using UnityEngine;

public class InputDispatcherService : MonoBehaviour
{
    private InputSystem_Actions input;

    private void Awake()
    {
        input = new InputSystem_Actions();
        input.Enable();
    }

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

    private void OnDisable()
    {
        input.Disable();
    }

    private bool isPaused = false;

    private void TogglePause()
    {
        isPaused = !isPaused;
        InputEventChannel.RaisePauseToggled(isPaused);
    }
}
