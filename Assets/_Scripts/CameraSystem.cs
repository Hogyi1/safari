using Unity.Cinemachine;
using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cinemachineCamera;
    private CinemachineFollow followComponent;

    [Header("Enable/disable Features")]
    [SerializeField, Tooltip("Select to enable edge scrolling.")]
    private bool useEdgeScrolling = false;

    [Header("Edge Scrolling")]
    [SerializeField, Tooltip("Distance from the screen edge before triggering edge scrolling.")]
    private int edgeScrollSize = 20;

    [Header("Camera Movement")]
    [SerializeField, Tooltip("Speed at which the camera moves around.")]
    private float moveSpeed = 20f;

    [SerializeField, Tooltip("Speed at which the camera rotates.")]
    private float rotateSpeed = 1f;

    [SerializeField, Tooltip("Speed at which the camera moves up and down.")]
    private float heightSpeed = 10f;

    [Header("Zoom Settings")]
    [SerializeField, Tooltip("Speed of zooming in and out.")]
    private float zoomSpeed = 10f;

    [SerializeField, Tooltip("Amount of zoom per step.")]
    private float zoomAmount = 3f;

    [SerializeField, Tooltip("Minimum distance the camera can zoom in.")]
    private float followOffsetMin = 13f;

    [SerializeField, Tooltip("Maximum distance the camera can zoom out.")]
    private float followOffsetMax = 35f;

    private Vector3 followOffset;

    private void Awake()
    {
        followComponent = (CinemachineFollow)cinemachineCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
    }

    private void Update()
    {
        HandleCameraMovement();
        if (useEdgeScrolling) HandleCameraMovementEdgeScrolling();
        HandleCameraRotation();
        HandleCameraZoom();

        /* This feature needs more work...
        HandleCameraHeight();
        */
    }

    private void HandleCameraMovement()
    {
        Vector3 inputDir = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.W)) inputDir.z = +1f;
        if (Input.GetKey(KeyCode.S)) inputDir.z = -1f;
        if (Input.GetKey(KeyCode.A)) inputDir.x = -1f;
        if (Input.GetKey(KeyCode.D)) inputDir.x = +1f;

        Vector3 moveDir = transform.forward * inputDir.z + transform.right * inputDir.x;

        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }

    private void HandleCameraMovementEdgeScrolling()
    {
        Vector3 inputDir = new Vector3(0, 0, 0);

        if (Input.mousePosition.x < edgeScrollSize) inputDir.x = -1f;
        if (Input.mousePosition.y < edgeScrollSize) inputDir.z = -1f;
        if (Input.mousePosition.x > Screen.width - edgeScrollSize) inputDir.x = +1f;
        if (Input.mousePosition.y > Screen.height - edgeScrollSize) inputDir.z = +1f;

        Vector3 moveDir = transform.forward * inputDir.z + transform.right * inputDir.x;

        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }

    private void HandleCameraRotation()
    {
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X") * rotateSpeed;

            transform.Rotate(Vector3.up * mouseX, Space.World);
        }
    }

    private void HandleCameraHeight()
    {
        float heightDir = 0f;
        if (Input.GetKey(KeyCode.Q) && transform.position.y >= -15f) heightDir = -1f;
        if (Input.GetKey(KeyCode.E) && transform.position.y <= 35f) heightDir = +1f;

        transform.position += new Vector3(0, heightDir * heightSpeed * Time.deltaTime, 0);
    }

    private void HandleCameraZoom()
    {
        if (Input.mouseScrollDelta.y > 0)
        {
            followOffset.y -= zoomAmount;
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            followOffset.y += zoomAmount;
        }

        followOffset.y = Mathf.Clamp(followOffset.y, followOffsetMin, followOffsetMax);

        followComponent.FollowOffset = Vector3.Lerp(followComponent.FollowOffset, followOffset, zoomSpeed * Time.deltaTime);
    }
}
