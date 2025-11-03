using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.UI;

/// <summary>
/// Updates the virtual cursor's position on the UI canvas based on low-level
/// input data and maintains proper UI layering and scaling.
/// </summary>
public class VirtualCursorView : MonoBehaviour
{
    /// <summary>
    /// RectTransform of the UI canvas, used to adjust the cursor's scale relative to the canvas.
    /// </summary>
    [SerializeField] private RectTransform canvasRectTransform;

    /// <summary>
    /// Component providing low-level virtual mouse input state.
    /// </summary>
    public VirtualMouseInput virtualMouseInput;

    /// <summary>
    /// Initializes the reference to the VirtualMouseInput component on this GameObject.
    /// </summary>
    private void Awake()
    {
        virtualMouseInput = GetComponent<VirtualMouseInput>();
    }

    /// <summary>
    /// Adjusts the cursor's local scale to compensate for canvas scaling
    /// and ensures the cursor is rendered on top of other UI elements.
    /// </summary>
    private void Update()
    {
        transform.localScale = Vector3.one * (1f / canvasRectTransform.localScale.x);
        transform.SetAsLastSibling();
    }

    /// <summary>
    /// Reads the virtual mouse position, clamps it to the screen bounds,
    /// and updates the low-level input state accordingly.
    /// </summary>
    private void LateUpdate()
    {
        Vector2 virtualMousePosition = virtualMouseInput.virtualMouse.position.value;
        virtualMousePosition.x = Mathf.Clamp(virtualMousePosition.x, 0f, Screen.width);
        virtualMousePosition.y = Mathf.Clamp(virtualMousePosition.y, 0f, Screen.height);
        UnityEngine.InputSystem.LowLevel.InputState.Change(virtualMouseInput.virtualMouse, virtualMousePosition);
    }
}
