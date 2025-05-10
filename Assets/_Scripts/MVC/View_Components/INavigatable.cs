using System.Collections.Generic;
using UnityEngine;

public interface INavigatable
{
    public void SetTarget(Vector3 position);
    public void SetWayPoints(List<Vector3> waypoints);
    public void Follow(MonoBehaviour targetView);
    public void StopMovementInstantly();
    public void StopMovement();
    public void ResetMovement();
}
