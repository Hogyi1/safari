using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines a set of methods for navigation behavior using a target position, multiple waypoints, or a follow target.
/// </summary>
public interface INavigatable
{
    /// <summary>
    /// Sets a single target position for navigation.
    /// </summary>
    /// <param name="position">The world-space position to navigate to.</param>
    public void SetTarget(Vector3 position);

    /// <summary>
    /// Sets a list of waypoints to be visited sequentially.
    /// </summary>
    /// <param name="waypoints">A list of world-space positions to follow.</param>
    public void SetWayPoints(List<Vector3> waypoints);

    /// <summary>
    /// Follows a moving target, such as another MonoBehaviour object.
    /// </summary>
    /// <param name="targetView">The target to follow.</param>
    public void Follow(MonoBehaviour targetView);

    /// <summary>
    /// Instantly stops all navigation and disables movement.
    /// </summary>
    public void StopMovementInstantly();

    /// <summary>
    /// Stops movement without disabling the navigation component.
    /// </summary>
    public void StopMovement();

    /// <summary>
    /// Resumes movement if previously stopped.
    /// </summary>
    public void ResetMovement();
}
