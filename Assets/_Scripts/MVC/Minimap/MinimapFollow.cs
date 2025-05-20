using UnityEngine;

/// <summary>
/// Positions the minimap camera to follow a target from above,
/// with optional X and Z axis offsets and fixed height.
/// </summary>
public class MinimapFollow : MonoBehaviour
{
    /// <summary>
    /// The target Transform the camera should follow (e.g. player).
    /// </summary>
    [SerializeField] private Transform target;

    /// <summary>
    /// Fixed height of the camera above the target.
    /// </summary>
    [SerializeField] private float height = 50f;

    /// <summary>
    /// Horizontal offset in the X-axis from the target's position.
    /// </summary>
    [SerializeField] private float offsetX = 0;

    /// <summary>
    /// Depth offset in the Z-axis from the target's position.
    /// </summary>
    [SerializeField] private float offsetZ = 0;

    /// <summary>
    /// Updates the camera position each frame after other updates.
    /// Keeps the camera above the target at a fixed height and offset.
    /// </summary>
    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 newPos = new Vector3(
            target.position.x + offsetX,
            height,
            target.position.z + offsetZ
        );
        transform.position = newPos;
    }
}
