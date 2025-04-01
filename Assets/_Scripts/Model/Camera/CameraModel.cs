using UnityEngine;

public class CameraModel : MonoBehaviour
{
    [Header("Edge Scrolling")]
    public bool useEdgeScrolling = false;
    public int edgeScrollSize = 20;

    [Header("Camera Movement")]
    public float moveSpeed = 20f;
    public float rotateSpeed = 1f;
    public float heightSpeed = 10f;

    [Header("Zoom Settings")]
    public float zoomSpeed = 10f;
    public float zoomAmount = 3f;
    public float followOffsetMin = 13f;
    public float followOffsetMax = 35f;

    [HideInInspector] public Vector3 followOffset;

    public void SetFollowOffsetY(float y)
    {
        followOffset = new Vector3(followOffset.x, y, followOffset.z);
    }
}
