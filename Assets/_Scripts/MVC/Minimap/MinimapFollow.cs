using UnityEngine;

public class MinimapFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float height = 50f;

    // → Két új Inspector-paraméter, amikkel X és Z irányban tolod a kamerát
    [SerializeField] private float offsetX = 0;
    [SerializeField] private float offsetZ = 0;

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
