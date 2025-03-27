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

    [SerializeField]
    private LayerMask SelectableLayermask;

    private Vector3 LastPosition;

    public event Action OnClicked, OnExit;

    [SerializeField]
    private Material setMaterial;

    GameObject lastHitObject = null;

    bool isCoroutineStarted = false;

    public static InputManager Instance;
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
    }



    public bool IsPointerOverUI() => EventSystem.current.IsPointerOverGameObject();

    public BuildingView GetHoveredObjectScript()
    {
        Vector3 mousePos = Input.mousePosition;
        Ray ray = SceneCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 999f, SelectableLayermask))
        {
            return hit.collider.GetComponentInParent<BuildingView>();
        }

        return null;
    }

    public Vector3 GetSelectedMapPosition()
    {
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
