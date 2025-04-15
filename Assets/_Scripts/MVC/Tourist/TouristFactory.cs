using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class TouristFactory : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> TouristPrefabs;

    [SerializeField] private GameObject Entrance;

    List<string> clothes = new List<string> { "Shirt", "Pants", "Shoes" };

    [SerializeField] private List<Color> skinColors;

    [SerializeField] private List<Color> hairColors;

    public TouristView CreateTourist(Tourist newTourist)
    {
        GameObject prefab = GetRandomPrefab();

        GameObject instance = Instantiate(prefab, Entrance.transform.position, Quaternion.identity);
        TouristView view = instance.GetComponent<TouristView>();
        view.Init(newTourist);

        SetMaterials(GetMaterials(instance, clothes));

        Color selectedColor = skinColors[Random.Range(0, skinColors.Count)];
        foreach (Material mat in GetMaterials(instance, new List<string> { "Skin" }))
        {
            mat.color = selectedColor;
        }
        GetMaterial(instance, "Hair").color = hairColors[Random.Range(0, hairColors.Count)];

        return view;

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
