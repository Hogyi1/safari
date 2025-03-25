using UnityEngine;

public interface IBuildingState
{
    void EndState();
    void OnAction(Vector3 gridPosition);
    void UpdateState(Vector3 gridPosition);
}