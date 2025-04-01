
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PreviewSystem : MonoBehaviour
{
    [SerializeField]
    private float previewYOffset = 0.2f;

    [SerializeField]
    private GameObject cellIndicator;
    private GameObject previewObject;

    [SerializeField]
    private Material previewMaterialPrefab;
    private Material previewMaterialInstance;

    [SerializeField]
    private GameObject gridVisualization;

    private Renderer cellIndicatorRenderer;

    [SerializeField]
    private Color WrongColor;

    [SerializeField]
    private Color ValidColor;

    private void Start()
    {
        previewMaterialInstance = new Material(previewMaterialPrefab);
        gridVisualization.SetActive(false);
        cellIndicator.SetActive(false);
        cellIndicatorRenderer = cellIndicator.GetComponentInChildren<Renderer>();
    }

    void Update()
    {
        if (previewObject != null)
        {
            float t = 0.5f + 0.2f * Mathf.Sin(Time.unscaledTime * 5f);

            Color color = previewMaterialInstance.color;
            color.a = t;
            previewMaterialInstance.color = color;
        }
    }

    public void SetGridSize(float size)
    {
        Material mat = gridVisualization.GetComponent<DecalProjector>().material;

        mat.SetVector("_Size", new Vector4(size, size, 0f, 0f));
        mat.SetFloat("_Thickness", size < 1f ? 0.04f : 0.1f);
    }
    public void StartShowingPlacementPreview(GameObject prefab, Vector2Int size)
    {
        gridVisualization.SetActive(true);
        previewObject = Instantiate(prefab);
        PreparePreview(previewObject);
        PrepareCursor(size);
        cellIndicator.SetActive(true);
    }

    private void PrepareCursor(Vector2Int size)
    {
        if (size.x > 0 || size.y > 0)
        {
            cellIndicator.transform.localScale = new Vector3(size.x, 1, size.y);
            cellIndicatorRenderer.material.mainTextureScale = size;
        }
    }

    private void PreparePreview(GameObject previewObject)
    {
        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = previewMaterialInstance;
            }
            renderer.materials = materials;
        }
    }

    public void StopShowingPreview()
    {
        cellIndicator.SetActive(false);
        gridVisualization.SetActive(false);
        if (previewObject != null)
            Destroy(previewObject);
    }

    public void UpdatePosition(Vector3 position, bool validity)
    {
        if (previewObject != null)
        {
            MovePreview(position);
            ApplyFeedbackToPreview(validity);
        }

        MoveCursor(position);
        ApplyFeedbackToCursor(validity);
    }

    private void ApplyFeedbackToPreview(bool validity)
    {
        Color c = validity ? ValidColor : WrongColor;

        c.a = 0.5f;
        previewMaterialInstance.color = c;
    }

    private void ApplyFeedbackToCursor(bool validity)
    {
        Color c = validity ? ValidColor : WrongColor;

        c.a = 0.5f;
        cellIndicatorRenderer.material.color = c;
    }

    private void MoveCursor(Vector3 position)
    {
        cellIndicator.transform.position = new Vector3(
            position.x,
            position.y + previewYOffset,
            position.z);
    }

    private void MovePreview(Vector3 position)
    {
        previewObject.transform.position = new Vector3(
            position.x,
            position.y + previewYOffset,
            position.z);
    }

    internal void StartShowingRemovePreview()
    {
        cellIndicator.SetActive(true);
        PrepareCursor(Vector2Int.one);
        ApplyFeedbackToCursor(false);
    }
}
