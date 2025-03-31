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

    private enum InputDeviceType { None, MouseKeyboard, Gamepad }
    private InputDeviceType lastUsedDevice = InputDeviceType.None;

    private void OnEnable()
    {
        moveAction.action.Enable();
        rotateAction.action.Enable();
        heightAction.action.Enable();
        zoomScrollAction.action.Enable();
        zoomTriggerAction.action.Enable();

        moveAction.action.performed += UpdateLastUsedDevice;
        rotateAction.action.performed += UpdateLastUsedDevice;
        heightAction.action.performed += UpdateLastUsedDevice;
        zoomScrollAction.action.performed += UpdateLastUsedDevice;
        zoomTriggerAction.action.performed += UpdateLastUsedDevice;
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        rotateAction.action.Disable();
        heightAction.action.Disable();
        zoomScrollAction.action.Disable();
        zoomTriggerAction.action.Disable();
    }

    private void UpdateLastUsedDevice(InputAction.CallbackContext ctx)
    {
        if (ctx.control.device is Gamepad)
            lastUsedDevice = InputDeviceType.Gamepad;
        else if (ctx.control.device is Keyboard || ctx.control.device is Mouse)
            lastUsedDevice = InputDeviceType.MouseKeyboard;
    }

    private void Update()
    {
        HandleMovement();
        if (model.useEdgeScrolling) HandleEdgeScrolling();
        HandleRotation();
        HandleHeight();
        HandleZoom();
    }

    private void HandleMovement()
    {
        Vector2 input = moveAction.action.ReadValue<Vector2>();
        Vector3 moveDir = view.transform.forward * input.y + view.transform.right * input.x;
        view.Move(moveDir * model.moveSpeed * Time.deltaTime);
    }

    private void HandleEdgeScrolling()
    {
        Vector3 inputDir = Vector3.zero;

        if (Mouse.current.position.ReadValue().x < model.edgeScrollSize) inputDir.x = -1f;
        if (Mouse.current.position.ReadValue().y < model.edgeScrollSize) inputDir.z = -1f;
        if (Mouse.current.position.ReadValue().x > Screen.width - model.edgeScrollSize) inputDir.x = 1f;
        if (Mouse.current.position.ReadValue().y > Screen.height - model.edgeScrollSize) inputDir.z = 1f;

        Vector3 moveDir = view.transform.forward * inputDir.z + view.transform.right * inputDir.x;
        view.Move(moveDir * model.moveSpeed * Time.deltaTime);
    }

    private void HandleRotation()
    {
        if (lastUsedDevice == InputDeviceType.MouseKeyboard)
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
        else if (lastUsedDevice == InputDeviceType.Gamepad)
        {
            Vector2 rotateInput = rotateAction.action.ReadValue<Vector2>();
            view.Rotate(rotateInput.x * model.rotateSpeed);
        }
    }

    private void HandleHeight()
    {
        float heightInput = heightAction.action.ReadValue<float>();
        view.Move(new Vector3(0, heightInput * model.heightSpeed * Time.deltaTime, 0));
    }

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
