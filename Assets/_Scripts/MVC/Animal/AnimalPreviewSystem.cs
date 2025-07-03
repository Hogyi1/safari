using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AnimalPreviewSystem : MonoBehaviour
{
    [SerializeField] private DecalProjector projector;
    [SerializeField] private NavMeshSurface animalSurface;
    [SerializeField] private Color wrong;
    [SerializeField] private Color correct;
    [SerializeField] private float yOffset;
    private bool isShowing;

    void Start()
    {
        isShowing = false;
        projector.gameObject.SetActive(false);
    }

    public void StartShowingPreview()
    {
        isShowing = true;
        projector.gameObject.SetActive(true);
    }

    public void StopShowingPreview()
    {
        isShowing = false;
        projector.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isShowing) return;

        Vector3 mousePos = InputManager.Instance.GetSelectedMapPosition();
        float y = Terrain.activeTerrain.SampleHeight(mousePos) + yOffset;
        Vector3 pos = new Vector3(mousePos.x, y, mousePos.z);
        bool canPlace = CheckValidity(new Vector3(mousePos.x, 0, mousePos.z));
        projector.material.color = canPlace ? correct : wrong;
        projector.transform.position = pos;
    }

    public bool CheckValidity(Vector3 position)
    {
        UnityEngine.AI.NavMeshHit hit;
        float maxDistance = 1f;

        return UnityEngine.AI.NavMesh.SamplePosition(position, out hit, maxDistance, UnityEngine.AI.NavMesh.AllAreas);
    }
}
