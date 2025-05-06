using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Detects hover and click interactions on scene objects and notifies interactable views.
/// </summary>
public class InteractableViewHandler : MonoBehaviour
{
    /// <summary>
    /// Camera used to raycast from screen to world objects.
    /// </summary>
    [SerializeField]
    private Camera SceneCamera;

    /// <summary>
    /// Layer mask for selectable interactable objects.
    /// </summary>
    [SerializeField] private LayerMask SelectableLayermask;

    /// <summary>
    /// Layer mask for road objects, treated as interactable.
    /// </summary>
    [SerializeField] private LayerMask RoadLayerMask;

    /// <summary>
    /// The last interactable object that was hovered.
    /// </summary>
    private IInteractable LastView;

    /// <summary>
    /// The currently active interactable view after click.
    /// </summary>
    private IInteractable ActiveView;

    /// <summary>
    /// Subscribes to click events on start.
    /// </summary>
    void Start()
    {
        InputManager.Instance.OnClicked += HandleClick;
    }

    /// <summary>
    /// Updates hover state each frame and invokes OnHover/OnExit accordingly.
    /// </summary>
    void Update()
    {
        IInteractable hoveredObject = GetHoveredObjectScript();
        if (hoveredObject != LastView)
        {
            if (!LastView.IsUnityNull())
            {
                LastView.OnExit();
            }
            LastView = hoveredObject;

            if (!LastView.IsUnityNull())
            {
                LastView.OnHover();
            }
        }

        if (hoveredObject == null && !LastView.IsUnityNull())
        {
            LastView.OnExit();
        }

    }

    /// <summary>
    /// Handles click events, activating or deactivating the interactable view.
    /// </summary>
    private void HandleClick()
    {
        if (LastView != null)
        {
            SetViewActive();
        }
        else
        {
            if (!IsPointerOverUI()) SetViewInactive();
        }
    }

    /// <summary>
    /// Ensures active view is cleared when handler is disabled.
    /// </summary>
    private void OnDisable()
    {
        SetViewInactive();
    }

    /// <summary>
    /// Determines if the pointer is currently over a UI element.
    /// </summary>
    /// <returns>True if over UI; otherwise false.</returns>
    public bool IsPointerOverUI() => EventSystem.current.IsPointerOverGameObject();

    /// <summary>
    /// Raycasts into the scene to find the IInteractable component under the cursor.
    /// </summary>
    /// <returns>The hovered IInteractable or null if none.</returns>
    public IInteractable GetHoveredObjectScript()
    {
        if (IsPointerOverUI())
            return null;

        Vector3 mousePos = Input.mousePosition;
        Ray ray = SceneCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 999f, SelectableLayermask | RoadLayerMask))
        {
            return hit.collider.GetComponentInParent<IInteractable>();
        }

        return null;
    }

    /// <summary>
    /// Deactivates the currently active interactable view and hides its popup.
    /// </summary>
    public void SetViewInactive()
    {
        if (!ActiveView.IsUnityNull())
        {
            PopupManager.Instance.HidePopup();
            ActiveView.OnCancel();
            ActiveView = null;
        }
    }

    /// <summary>
    /// Activates the last hovered view, canceling any previous active view.
    /// </summary>
    private void SetViewActive()
    {
        if (!LastView.IsUnityNull())
        {
            SetViewInactive();
            ActiveView = LastView;
            ActiveView.OnAction();
        }
    }
}
