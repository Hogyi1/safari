using UnityEngine;

/// <summary>
/// Keeps a minimap icon positioned and rotated above a target Transform.
/// The icon follows the XZ position of the target and stays at a fixed height.
/// </summary>
public class MinimapIconFollow : MonoBehaviour
{
    /// <summary>
    /// The target Transform the icon should follow (e.g. a player or unit).
    /// </summary>
    [SerializeField] private Transform target;

    /// <summary>
    /// The fixed Y-axis height at which the minimap icon should float.
    /// </summary>
    [SerializeField] private float height;

    /// <summary>
    /// Updates the icon’s position and rotation each frame after all Updates.
    /// Ensures the icon follows the target on the XZ plane and rotates to match orientation.
    /// </summary>
    private void LateUpdate()
    {
        Vector3 newPos = new Vector3(
            target.position.x,
            height,
            target.position.z
        );
        transform.position = newPos;

        // Aligns the icon with the target's facing direction on the minimap (top-down)
        transform.rotation = Quaternion.Euler(90f, target.eulerAngles.y + 270f, 0f);
    }
}
