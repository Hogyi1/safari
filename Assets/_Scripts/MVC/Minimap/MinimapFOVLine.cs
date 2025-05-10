using UnityEngine;

public class MinimapFOVLine : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;   // a player nézőkamerája
    [SerializeField] private float viewDistance = 5f;  // milyen hosszú legyen a vonal
    private LineRenderer lr;
    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.useWorldSpace = false;
        lr.positionCount = 4;
        if (playerCamera == null)
            playerCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (playerCamera == null) return;

        // 1) Forgás: csak a Y tengelyt másoljuk
        float yaw = playerCamera.transform.eulerAngles.y;
        transform.localRotation = Quaternion.Euler(90f, yaw, 0f);

        // 2) FOV félszög radiánban
        float halfRad = playerCamera.fieldOfView * 0.5f * Mathf.Deg2Rad;

        // 3) Két irányvektor a minimap síkjában
        Vector3 rightDir = new Vector3(Mathf.Sin(halfRad), 0, Mathf.Cos(halfRad)) * viewDistance;
        Vector3 leftDir = new Vector3(-Mathf.Sin(halfRad), 0, Mathf.Cos(halfRad)) * viewDistance;

        // 4) V-alak kirajzolása: origin→jobb, origin→bal
        lr.SetPosition(0, Vector3.zero);
        lr.SetPosition(1, rightDir);
        lr.SetPosition(2, Vector3.zero);
        lr.SetPosition(3, leftDir);
    }
}
