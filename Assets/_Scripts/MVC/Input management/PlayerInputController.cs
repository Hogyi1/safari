using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    private InputSystem_Actions input;

    [SerializeField] private PlayerInput playerInput;

    private bool uiModeActive = false;

    private void Awake()
    {
        input = new InputSystem_Actions();
        input.Enable();
    }

    private void OnEnable()
    {
        input.Global.ToggleUI.performed += ctx => ToggleUI();
    }

    private void OnDisable()
    {
        input.Disable();
    }

    private void ToggleUI()
    {
        SetUIMode(!uiModeActive);
    }

    public void SetUIMode(bool active)
    {
        uiModeActive = active;
        playerInput.enabled = !active;

        InputEventChannel.RaiseToggleUIMode(active);
    }
}
