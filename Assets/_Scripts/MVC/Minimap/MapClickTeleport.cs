using UnityEngine;
using UnityEngine.EventSystems;

public class MapClickTeleport : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Camera worldMapCamera;     // A worldmapet renderelő kamera
    [SerializeField] private Transform playerTransform; // A teleportálandó cél (pl. CameraFocus)
    [SerializeField] private RectTransform mapRect;     // A RawImage RectTransform-ja
    [SerializeField] private GameObject worldMapUI;     // A teljes UI panel (RawImage vagy parent Canvas)

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (worldMapUI != null && worldMapUI.activeSelf)
            {
                worldMapUI.SetActive(false);
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Kattintás történt a térképen.");

        Vector2 localPoint;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(mapRect, eventData.position, null, out localPoint))
        {
            Debug.LogWarning("Nem sikerült átalakítani a kattintási pozíciót.");
            return;
        }

        Vector2 normalized = RectPointToNormalized(localPoint, mapRect);
        Vector3 worldPos = NormalizedToWorldPosition(normalized);

        if (playerTransform != null)
        {
            Debug.Log("Teleportálás pozícióra: " + worldPos);
            playerTransform.position = new Vector3(worldPos.x, playerTransform.position.y, worldPos.z);

            if (worldMapUI != null)
            {
                worldMapUI.SetActive(false);
            }
        }
        else
        {
            Debug.LogWarning("Nincs beállítva playerTransform!");
        }
    }

    private Vector2 RectPointToNormalized(Vector2 localPoint, RectTransform rect)
    {
        float x = (localPoint.x + rect.rect.width / 2f) / rect.rect.width;
        float y = (localPoint.y + rect.rect.height / 2f) / rect.rect.height;
        return new Vector2(x, y);
    }

    private Vector3 NormalizedToWorldPosition(Vector2 normalized)
    {
        float orthoHeight = worldMapCamera.orthographicSize * 2f;
        float orthoWidth = orthoHeight * worldMapCamera.aspect;

        Vector3 bottomLeft = worldMapCamera.transform.position - new Vector3(orthoWidth / 2f, 0, orthoHeight / 2f);
        return bottomLeft + new Vector3(normalized.x * orthoWidth, 0, normalized.y * orthoHeight);
    }
}
