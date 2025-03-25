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



        //if (Physics.Raycast(ray, out hit, 999f, SelectableLayermask))
        //{

        //    // Próbáljuk meg kiolvasni a rajta lévő "Building" komponenst
        //    BuildingView building = hitObject.GetComponentInParent<BuildingView>();
        //    if (building != null && lastHitObject != hitObject && !isCoroutineStarted)
        //    {
        //        isCoroutineStarted = true;
        //        foreach (Transform t in hitObject.transform)
        //        {
        //            StartCoroutine(PreparePreview(t.gameObject));
        //        }
        //        Debug.Log(hitObject);
        //        // hitObject.GetComponent<Renderer>().material = materials;
        //        lastHitObject = hitObject;
        //    }
        //}
        //else if (lastHitObject != null && !isCoroutineStarted)
        //{
        //    isCoroutineStarted = true;
        //    foreach (Transform t in lastHitObject.transform)
        //    {
        //        StartCoroutine(ResetObject(t.gameObject));
        //    }

        //    lastHitObject = null;
        //}
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
        // mousePos.z = SceneCamera.nearClipPlane;
        Ray ray = SceneCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, PlacementLayermask))
        {
            LastPosition = hit.point;
        }
        return LastPosition;

    }
}
