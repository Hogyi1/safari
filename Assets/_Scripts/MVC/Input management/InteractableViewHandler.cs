using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class InteractableViewHandler : MonoBehaviour
{
    [SerializeField]
    private Camera SceneCamera;

    [SerializeField] private LayerMask SelectableLayermask;
    [SerializeField] private LayerMask RoadLayerMask;

    private IInteractable LastView;
    private IInteractable ActiveView;

    void Start()
    {
        InputManager.Instance.OnClicked += HandleClick;
    }

    // Update is called once per frame
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
    // Kezeli a kattintást, a View-t aktiválja
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

    private void OnDisable()
    {
        SetViewInactive();
    }

    // UI felett van?
    public bool IsPointerOverUI() => EventSystem.current.IsPointerOverGameObject();

    // Visszaadja a lehoverelt objektum scriptjét
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

    // Inaktívvá állítja a jelenlegi View-t
    public void SetViewInactive()
    {
        if (!ActiveView.IsUnityNull())
        {
            PopupManager.Instance.HidePopup();
            ActiveView.OnCancel();
            ActiveView = null;
        }
    }

    // Aktiválja a jelenlegi View-t és a popupot is
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
