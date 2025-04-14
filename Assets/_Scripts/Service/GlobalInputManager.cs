using UnityEngine;

public class GlobalInputManager : MonoBehaviour
{
    private InputSystem_Actions input;

    private void Awake()
    {
        input = new InputSystem_Actions();
        input.Enable();
    }

    private void OnEnable()
    {
        // Player Action Map
        // input.Player.Move.performed += ctx => InputEventChannel.RaiseMove(ctx.ReadValue<Vector2>());
        input.Player.Look.performed += ctx => InputEventChannel.RaiseLook(ctx.ReadValue<Vector2>());
        // input.Player.Attack.performed += _ => InputEventChannel.RaiseClick();

        // UI Action Map
        input.UI.RightClick.performed += _ => InputEventChannel.RaiseRightClick();

        // CameraControls Action Map
        input.CameraControls.Move.performed += ctx => InputEventChannel.RaiseCameraMove(ctx.ReadValue<Vector2>());
        input.CameraControls.Rotate.performed += ctx => InputEventChannel.RaiseCameraRotate(ctx.ReadValue<Vector2>());
        input.CameraControls.Height.performed += ctx => InputEventChannel.RaiseCameraHeight(ctx.ReadValue<float>());
        input.CameraControls.ZoomScroll.performed += ctx => InputEventChannel.RaiseZoom(ctx.ReadValue<float>());
        input.CameraControls.ZoomTriggers.performed += ctx => InputEventChannel.RaiseZoom(ctx.ReadValue<float>());
    }

    private void OnDisable()
    {
        input.Disable();
    }
}
