using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class CameraView : MonoBehaviour, IDataPersistence
{
    [SerializeField] private CinemachineCamera cinemachineCamera;
    private CinemachineFollow followComponent;

    public float Priority => 3000f;

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
    /// Directly set the camera’s world position.
    /// </summary>
    public void SetPosition(Vector3 newWorldPos)
    {
        transform.position = newWorldPos;
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

    public IEnumerator LoadData(GameData data)
    {
        gameObject.transform.position = data.cameraSaveData.CameraPosition;
        gameObject.transform.rotation = Quaternion.Euler(data.cameraSaveData.CameraRotation);
        yield return null;
    }

    public void SaveData(GameData data)
    {
        data.cameraSaveData = new CameraSaveData(gameObject.transform.position, gameObject.transform.rotation.eulerAngles);
    }

}
