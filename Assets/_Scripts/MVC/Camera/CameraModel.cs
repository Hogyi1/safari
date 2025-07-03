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

    [Header("Terrain Bounds (for clamping)")]
    [Tooltip("The Terrain whose size defines our allowed X/Z range.")]
    public Terrain terrain;

    private float _minX, _maxX, _minZ, _maxZ;

    [HideInInspector] public Vector3 followOffset;

    private void Awake()
    {
        // Cache terrain bounds
        Vector3 tPos = terrain.transform.position;
        Vector3 tSize = terrain.terrainData.size;
        _minX = tPos.x;
        _minZ = tPos.z;
        _maxX = tPos.x + tSize.x;
        _maxZ = tPos.z + tSize.z;
    }

    /// <summary>
    /// Returns the input position with X/Z clamped inside the terrain bounds.
    /// Y (height) is left unchanged.
    /// </summary>
    public Vector3 ClampPosition(Vector3 worldPos)
    {
        worldPos.x = Mathf.Clamp(worldPos.x, _minX, _maxX);
        worldPos.z = Mathf.Clamp(worldPos.z, _minZ, _maxZ);
        return worldPos;
    }

    /// <summary>
    /// Update only the Y component of the follow-offset, preserving X and Z.
    /// </summary>
    /// <param name="y">New vertical offset value (clamped by controller).</param>
    public void SetFollowOffsetY(float y)
    {
        followOffset = new Vector3(followOffset.x, y, followOffset.z);
    }
}
