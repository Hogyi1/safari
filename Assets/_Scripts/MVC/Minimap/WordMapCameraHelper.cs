using UnityEngine;

/// <summary>
/// Automatically positions and configures a top-down orthographic camera
/// to fit the entire world map area based on the specified map size.
/// </summary>
public class WordMapCameraHelper : MonoBehaviour
{
    /// <summary>
    /// Reference to the camera that should be positioned. Defaults to Camera.main if not set.
    /// </summary>
    [SerializeField] private Camera worldMapCamera;

    /// <summary>
    /// The size of the map in world units. X = width, Y = depth.
    /// </summary>
    [SerializeField] private Vector2 mapSize = new Vector2(500, 500);

    /// <summary>
    /// Sets the camera position, rotation, and orthographic size to fully display the map area.
    /// Called on Start().
    /// </summary>
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
