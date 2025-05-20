using UnityEngine;

using UnityEngine.EventSystems;

/// <summary>
/// Handles teleporting a target transform (e.g. player or camera focus)
/// to a clicked position on a 2D world map UI (e.g. RawImage).
/// Converts screen clicks to world coordinates using an orthographic camera.
/// </summary>
public class MapClickTeleport : MonoBehaviour, IPointerClickHandler
{
    /// <summary>
    /// The camera rendering the world map (must be orthographic).
    /// </summary>
    [SerializeField] private Camera worldMapCamera;

    /// <summary>
    /// The transform to teleport when the map is clicked (e.g., player or camera focus).
    /// </summary>
    [SerializeField] private Transform playerTransform;

    /// <summary>
    /// The RectTransform of the UI element representing the map (e.g., RawImage).
    /// </summary>
    [SerializeField] private RectTransform mapRect;

    /// <summary>
    /// The full UI panel containing the map (can be used for validation or future toggling).
    /// </summary>
    [SerializeField] private GameObject worldMapUI;

    /// <summary>
    /// Handles pointer click events on the map UI.
    /// Converts the click to a world position and teleports the target transform.
    /// </summary>
    /// <param name="eventData">Pointer click data from Unity UI event system.</param>
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Map click detected.");

        Vector2 localPoint;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(mapRect, eventData.position, null, out localPoint))
        {
            Debug.LogWarning("Failed to convert screen point to local point.");
            return;
        }

        Vector2 normalized = RectPointToNormalized(localPoint, mapRect);
        Vector3 worldPos = NormalizedToWorldPosition(normalized);

        if (playerTransform != null)
        {
            Debug.Log("Teleporting to position: " + worldPos);
            playerTransform.position = new Vector3(worldPos.x, playerTransform.position.y, worldPos.z);
        }
        else
        {
            Debug.LogWarning("playerTransform is not assigned!");
        }
    }

    /// <summary>
    /// Converts a local point in the RectTransform to a normalized (0–1) coordinate.
    /// </summary>
    /// <param name="localPoint">The point relative to the RectTransform center.</param>
    /// <param name="rect">The RectTransform reference.</param>
    /// <returns>Normalized point (x, y) in the range [0,1].</returns>
    private Vector2 RectPointToNormalized(Vector2 localPoint, RectTransform rect)
    {
        float x = (localPoint.x + rect.rect.width / 2f) / rect.rect.width;
        float y = (localPoint.y + rect.rect.height / 2f) / rect.rect.height;
        return new Vector2(x, y);
    }

    /// <summary>
    /// Converts a normalized map position to world coordinates using the orthographic camera.
    /// </summary>
    /// <param name="normalized">Normalized (0–1) coordinates.</param>
    /// <returns>World space position on the map.</returns>
    private Vector3 NormalizedToWorldPosition(Vector2 normalized)
    {
        float orthoHeight = worldMapCamera.orthographicSize * 2f;
        float orthoWidth = orthoHeight * worldMapCamera.aspect;

        Vector3 bottomLeft = worldMapCamera.transform.position - new Vector3(orthoWidth / 2f, 0, orthoHeight / 2f);
        return bottomLeft + new Vector3(normalized.x * orthoWidth, 0, normalized.y * orthoHeight);
    }
}
