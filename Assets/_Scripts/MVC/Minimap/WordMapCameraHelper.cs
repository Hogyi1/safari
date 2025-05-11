using UnityEngine;

public class WordMapCameraHelper : MonoBehaviour
{
    [SerializeField] private Camera worldMapCamera;
    [SerializeField] private Vector2 mapSize = new Vector2(500, 500); // X = width, Y = depth

    private void Start()
    {
        if (worldMapCamera == null) worldMapCamera = Camera.main;

        Vector3 center = new Vector3(mapSize.x / 2f, 100f, mapSize.y / 2f);
        worldMapCamera.transform.position = center;
        worldMapCamera.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        float aspect = (float)Screen.width / Screen.height;
        float requiredSize = Mathf.Max(mapSize.y / 2f, mapSize.x / (2f * aspect));
        worldMapCamera.orthographicSize = requiredSize;
    }
}
