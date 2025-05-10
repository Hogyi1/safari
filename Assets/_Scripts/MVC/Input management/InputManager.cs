using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Centralizes low-level input handling, click events, and placement mode state.
/// Implements a singleton for global access.
/// </summary>
public class InputManager : MonoBehaviour
{
    /// <summary>
    /// Camera used for raycasting placement positions.
    /// </summary>
    [SerializeField]
    private Camera SceneCamera;

    /// <summary>
    /// Layer mask to determine valid placement surfaces.
    /// </summary>
    [SerializeField]
    private LayerMask PlacementLayermask;

    private VirtualCursorView vcv;

    /// <summary>
    /// Last valid world position clicked for placement.
    /// </summary>
    private Vector3 LastPosition;

    /// <summary>
    /// Events invoked when the actions occur.
    /// </summary>
    public event Action StopPlacement;

    /// <summary>
    /// Current input state mode (placement or normal).
    /// </summary>
    public State state = State.NormalMode;

    /// <summary>
    /// Singleton instance of the InputManager.
    /// </summary>
    public static InputManager Instance;

    /// <summary>
    /// Handler for interactable view activation.
    /// </summary>
    [SerializeField] private InteractableViewHandler ViewHandler;

    /// <summary>
    /// Ensures singleton instance and persists across scenes.
    /// </summary>
    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        vcv = new VirtualCursorView();
    }

    /// <summary>
    /// Processes click, exit, and arrow key inputs each frame.
    /// </summary>
    void Update()
    {

    }

    public Vector3 GetUDCPosition()
    {
        if (InputDeviceDetector.LastUsedDevice == InputDeviceDetector.InputDeviceType.Gamepad)
            return vcv.virtualMouseInput.virtualMouse.position.value;
        return Input.mousePosition;

    }

    /// <summary>
    /// Sets the input state mode, toggling the ViewHandler accordingly.
    /// </summary>
    /// <param name="state">The new input state to apply.</param>
    public void SetState(State state)
    {
        this.state = state;
        if (state == State.PlacementMode) ViewHandler.gameObject.SetActive(false);
        else
        {
            StopPlacement?.Invoke();
            ViewHandler.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Deactivates any active interactable view via the ViewHandler.
    /// </summary>
    public void DisableView()
    {
        ViewHandler.SetViewInactive();
    }

    /// <summary>
    /// Determines if the pointer is currently over a UI element.
    /// </summary>
    /// <returns>True if pointer is over UI; otherwise false.</returns>
    public bool IsPointerOverUI() => EventSystem.current.IsPointerOverGameObject();

    /// <summary>
    /// Performs a raycast to obtain the world position for placement under the cursor.
    /// </summary>
    /// <returns>The last valid placement position in world coordinates.</returns>
    public Vector3 GetSelectedMapPosition()
    {
        if (IsPointerOverUI())
            return Vector3.zero;

        Vector3 mousePos = GetUDCPosition();
        Ray ray = SceneCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, PlacementLayermask))
        {
            LastPosition = hit.point;
        }
        return LastPosition;
    }
}

/// <summary>
/// Enumerates the input manager states.
/// </summary>
public enum State
{
    /// <summary>
    /// Mode where placement input is processed.
    /// </summary>
    PlacementMode,

    /// <summary>
    /// Default mode for normal interactions.
    /// </summary>
    NormalMode
}
