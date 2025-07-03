using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Factory class responsible for creating and configuring Tourist instances,
/// including randomizing appearance and handling instantiation logic.
/// </summary>
public class TouristFactory : MonoBehaviour
{
    [SerializeField] private List<GameObject> TouristPrefabs;

    [SerializeField] private GameObject Entrance;
    [SerializeField] private GameObject TouristParent;

    [SerializeField] List<string> clothes = new List<string> { "Shirt", "Pants", "Shoes" }; // Can be modified
    [SerializeField] private List<Color> skinColors;
    [SerializeField] private List<Color> hairColors;

    private Bounds entranceBounds;


    /// <summary>
    /// Initializes the bounds of the entrance object for random spawn positioning.
    /// </summary>
    private void Start()
    {
        entranceBounds = Entrance.GetComponent<Renderer>().bounds;
    }


    /// <summary>
    /// Creates a new tourist with randomized position and appearance.
    /// </summary>
    /// <param name="ID">Unique ID of the tourist.</param>
    /// <returns>A new <see cref="Tourist"/> instance.</returns>
    public Tourist CreateTourist(int ID)
    {
        GameObject prefab = GetRandomPrefab();

        float x = Random.Range(entranceBounds.min.x, entranceBounds.max.x);
        float z = Random.Range(entranceBounds.min.z, entranceBounds.max.z);
        float y = entranceBounds.center.y;
        Vector3 spawnPosition = new Vector3(x, y, z);
        TouristView view = CreateTouristVisual(spawnPosition);
        TouristModel model = new TouristModel(ID);

        return new Tourist(ID, model, view);
    }


    /// <summary>
    /// Creates a tourist from saved data, restoring position and state.
    /// </summary>
    /// <param name="touristData">Previously saved tourist data.</param>
    /// <returns>A reconstructed <see cref="Tourist"/> instance.</returns>
    public Tourist CreateTourist(TouristSaveData touristData)
    {
        Vector3 spawnPosition = touristData.CurrentPosition;

        TouristView view = CreateTouristVisual(spawnPosition);
        TouristModel model = new TouristModel(touristData);

        switch (model.State)
        {
            case TouristState.Walking:
                view.StartWalkingToCar(touristData.CurrentDestination);
                break;
            case TouristState.In_car:
            case TouristState.On_tour:
            case TouristState.Finished:
                view.gameObject.SetActive(false);
                break;
            default:
                break;
        }

        return new Tourist(touristData.ID, model, view);
    }


    /// <summary>
    /// Instantiates a tourist prefab at the given position and randomizes appearance.
    /// </summary>
    /// <param name="position">Spawn position.</param>
    /// <returns>The <see cref="TouristView"/> component of the spawned tourist.</returns>
    private TouristView CreateTouristVisual(Vector3 position)
    {
        GameObject prefab = GetRandomPrefab();
        GameObject instance = Instantiate(prefab, position, Quaternion.identity);
        instance.transform.SetParent(TouristParent.transform, true);

        // Színek beállítása
        SetMaterials(GetMaterials(instance, clothes));

        Color selectedColor = skinColors[Random.Range(0, skinColors.Count)];
        foreach (Material mat in GetMaterials(instance, new List<string> { "Skin" }))
        {
            mat.color = selectedColor;
        }
        GetMaterial(instance, "Hair").color = hairColors[Random.Range(0, hairColors.Count)];

        return instance.GetComponent<TouristView>();
    }


    /// <summary>
    /// Returns a list of materials from the given GameObject that match any of the provided names.
    /// </summary>
    /// <param name="prefab">The GameObject to search materials on.</param>
    /// <param name="names">List of material name patterns to match.</param>
    /// <returns>Matching materials.</returns>
    public List<Material> GetMaterials(GameObject prefab, List<string> names)
    {
        Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>();

        List<Material> returnMat = new List<Material>();

        foreach (Renderer ren in renderers)
        {
            foreach (Material mat in ren.materials)
            {
                if (names.Any(n => mat.name.Contains(n)))
                {
                    returnMat.Add(mat);
                }
            }
        }
        return returnMat;
    }


    /// <summary>
    /// Retrieves the first material from a GameObject matching the given name pattern.
    /// </summary>
    /// <param name="prefab">The GameObject to search.</param>
    /// <param name="name">Material name pattern to search for.</param>
    /// <returns>The matched material or null if not found.</returns>
    public Material GetMaterial(GameObject prefab, string name)
    {
        Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>();

        foreach (Renderer ren in renderers)
        {
            foreach (Material mat in ren.materials)
            {
                if (mat.name.Contains(name))
                {
                    return mat;
                }
            }
        }
        return null;
    }


    /// <summary>
    /// Assigns random colors to each material in the list.
    /// </summary>
    /// <param name="materials">List of materials to recolor.</param>
    public void SetMaterials(List<Material> materials)
    {
        foreach (Material mat in materials)
        {
            Color randomColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            mat.color = randomColor;
        }
    }


    /// <summary>
    /// Returns a random tourist prefab from the available list.
    /// </summary>
    /// <returns>A randomly selected prefab.</returns>
    public GameObject GetRandomPrefab() => TouristPrefabs[Random.Range(0, TouristPrefabs.Count)];
}
