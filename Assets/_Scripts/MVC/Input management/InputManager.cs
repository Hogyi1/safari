using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private Camera SceneCamera;

    [SerializeField]
    private LayerMask PlacementLayermask;

    private Vector3 LastPosition;

    public event Action OnClicked, OnExit, Left, Right;

    public State state = State.NormalMode;

    public static InputManager Instance;

    [SerializeField] private InteractableViewHandler ViewHandler;

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            OnClicked?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnExit?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Left?.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Right?.Invoke();
        }
    }

    public void SetState(State state)
    {
        this.state = state;
        if (state == State.PlacementMode) ViewHandler.gameObject.SetActive(false);
        else ViewHandler.gameObject.SetActive(true);
    }

    public void DisableView()
    {
        ViewHandler.SetViewInactive();
    }

    public bool IsPointerOverUI() => EventSystem.current.IsPointerOverGameObject();

    public Vector3 GetSelectedMapPosition()
    {
        if (IsPointerOverUI())
            return Vector3.zero;

        Vector3 mousePos = Input.mousePosition;
        Ray ray = SceneCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, PlacementLayermask))
        {
            LastPosition = hit.point;
        }
        return LastPosition;
    }
}

public enum State
{
    PlacementMode,
    NormalMode
}
