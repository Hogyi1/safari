using UnityEngine;

public class MinimapIconFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float height;

    private void LateUpdate()
    {
        Vector3 newPos = new Vector3(
            target.position.x,
            height,
            target.position.z
        );
        transform.position = newPos;
        transform.rotation = Quaternion.Euler(90f, target.eulerAngles.y + 270f, 0f);
    }
}
