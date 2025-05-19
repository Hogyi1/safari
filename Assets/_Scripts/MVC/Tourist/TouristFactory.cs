using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class TouristFactory : MonoBehaviour
{
    [SerializeField] private List<GameObject> TouristPrefabs;

    [SerializeField] private GameObject Entrance;
    [SerializeField] private GameObject TouristParent;
    private Bounds entranceBounds;

    List<string> clothes = new List<string> { "Shirt", "Pants", "Shoes" };

    [SerializeField] private List<Color> skinColors;

    [SerializeField] private List<Color> hairColors;

    private void Start()
    {
        entranceBounds = Entrance.GetComponent<Renderer>().bounds;
    }

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

    public void SetMaterials(List<Material> materials)
    {
        foreach (Material mat in materials)
        {
            Color randomColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            mat.color = randomColor;
        }
    }

    public GameObject GetRandomPrefab()
    {
        return TouristPrefabs[Random.Range(0, TouristPrefabs.Count)];
    }
}
