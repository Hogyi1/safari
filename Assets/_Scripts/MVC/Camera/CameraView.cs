using UnityEngine;
using Unity.Cinemachine;

public class CameraView : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cinemachineCamera;
    private CinemachineFollow followComponent;

    /// <summary>
    /// Cache the Cinemachine body component used for follow-offset adjustments.
    /// </summary>
    private void Awake()
    {
        followComponent = (CinemachineFollow)cinemachineCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
    }

    /// <summary>
    /// Translate the camera’s transform by a given delta in world space.
    /// </summary>
    /// <param name="delta">Movement vector in world units.</param>
    public void Move(Vector3 delta)
    {
        transform.position += delta;
    }

    /// <summary>
    /// Rotate the camera around the global up axis.
    /// </summary>
    /// <param name="delta">Rotation in degrees.</param>
    public void Rotate(float delta)
    {
        transform.Rotate(Vector3.up * delta, Space.World);
    }

    /// <summary>
    /// Smoothly interpolate the Cinemachine follow-offset toward a target value.
    /// </summary>
    /// <param name="offset">Desired follow-offset relative to the target.</param>
    /// <param name="zoomSpeed">Interpolation speed multiplier.</param>
    public void SetFollowOffset(Vector3 offset, float zoomSpeed)
    {
        followComponent.FollowOffset = Vector3.Lerp(followComponent.FollowOffset, offset, zoomSpeed * Time.deltaTime);
    }
}
