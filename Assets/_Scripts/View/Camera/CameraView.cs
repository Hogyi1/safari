using UnityEngine;
using Unity.Cinemachine;

public class CameraView : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cinemachineCamera;
    private CinemachineFollow followComponent;

    private void Awake()
    {
        followComponent = (CinemachineFollow)cinemachineCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
    }

    public void Move(Vector3 delta)
    {
        transform.position += delta;
    }

    public void Rotate(float delta)
    {
        transform.Rotate(Vector3.up * delta, Space.World);
    }

    public void SetFollowOffset(Vector3 offset, float zoomSpeed)
    {
        followComponent.FollowOffset = Vector3.Lerp(followComponent.FollowOffset, offset, zoomSpeed * Time.deltaTime);
    }
}
