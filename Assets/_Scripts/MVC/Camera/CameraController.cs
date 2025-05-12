using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("MVC Components")]
    [SerializeField] private CameraModel model;
    [SerializeField] private CameraView view;

    [Header("UI Components")]
    [SerializeField] private CursorView cursorView;

    [Header("Input Actions")]
    public InputActionReference moveAction;
    public InputActionReference rotateAction;
    public InputActionReference heightAction;
    public InputActionReference zoomScrollAction;
    public InputActionReference zoomTriggerAction;

    /// <summary>
    /// Enable all input actions when this component is enabled.
    /// </summary>
    private void OnEnable()
    {
        moveAction.action.Enable();
        rotateAction.action.Enable();
        heightAction.action.Enable();
        zoomScrollAction.action.Enable();
        zoomTriggerAction.action.Enable();
    }

    /// <summary>
    /// Disable all input actions when this component is disabled.
    /// </summary>
    private void OnDisable()
    {
        moveAction.action.Disable();
        rotateAction.action.Disable();
        heightAction.action.Disable();
        zoomScrollAction.action.Disable();
        zoomTriggerAction.action.Disable();
    }

    /// <summary>
    /// Main per-frame update: handle movement, edge scrolling, rotation, height, and zoom.
    /// </summary>
    private void Update()
    {
        HandleMovement();
        if (model.useEdgeScrolling) HandleEdgeScrolling();
        HandleRotation();
        HandleHeight();
        HandleZoom();
    }

    /// <summary>
    /// Move the camera based on WASD/joystick input.
    /// </summary>
    private void HandleMovement()
    {
        Vector2 input = moveAction.action.ReadValue<Vector2>();
        Vector3 moveDir = view.transform.forward * input.y + view.transform.right * input.x;
        view.Move(moveDir * model.moveSpeed * Time.unscaledDeltaTime);
    }

    /// <summary>
    /// Move the camera when the cursor is near the screen edges.
    /// </summary>
    private void HandleEdgeScrolling()
    {
        Vector3 inputDir = Vector3.zero;

        if (Mouse.current.position.ReadValue().x < model.edgeScrollSize) inputDir.x = -1f;
        if (Mouse.current.position.ReadValue().y < model.edgeScrollSize) inputDir.z = -1f;
        if (Mouse.current.position.ReadValue().x > Screen.width - model.edgeScrollSize) inputDir.x = 1f;
        if (Mouse.current.position.ReadValue().y > Screen.height - model.edgeScrollSize) inputDir.z = 1f;

        Vector3 moveDir = view.transform.forward * inputDir.z + view.transform.right * inputDir.x;
        view.Move(moveDir * model.moveSpeed * Time.unscaledDeltaTime);
    }

    /// <summary>
    /// Rotate the camera via right-mouse drag or gamepad input.
    /// </summary>
    private void HandleRotation()
    {
        if (InputDeviceDetector.LastUsedDevice == InputDeviceDetector.InputDeviceType.MouseKeyboard)
        {
            if (Mouse.current.rightButton.isPressed)
            {
                cursorView.SetLookCursor();
                Vector2 rotateInput = rotateAction.action.ReadValue<Vector2>();
                view.Rotate(rotateInput.x * model.rotateSpeed);
            }
            else
            {
                cursorView.SetDefaultCursor();
            }
        }
        else if (InputDeviceDetector.LastUsedDevice == InputDeviceDetector.InputDeviceType.Gamepad)
        {
            Vector2 rotateInput = rotateAction.action.ReadValue<Vector2>();
            view.Rotate(rotateInput.x * model.rotateSpeed);
        }
    }

    /// <summary>
    /// Move the camera up or down along its Y axis.
    /// </summary>
    private void HandleHeight()
    {
        float heightInput = heightAction.action.ReadValue<float>();
        view.Move(new Vector3(0, heightInput * model.heightSpeed * Time.unscaledDeltaTime, 0));
    }

    /// <summary>
    /// Zoom the camera by adjusting the Cinemachine follow-offset Y value.
    /// </summary>
    private void HandleZoom()
    {
        float scrollInput = zoomScrollAction.action.ReadValue<float>();
        float triggerInput = zoomTriggerAction.action.ReadValue<float>();
        float zoomInput = scrollInput + triggerInput;

        float newY = Mathf.Clamp(model.followOffset.y - zoomInput * model.zoomAmount, model.followOffsetMin, model.followOffsetMax);
        model.SetFollowOffsetY(newY);
        view.SetFollowOffset(model.followOffset, model.zoomSpeed);
    }
}
