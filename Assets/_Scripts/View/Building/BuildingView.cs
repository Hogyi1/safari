using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity;
using Unity.VisualScripting;
using UnityEngine;

public class BuildingView : MonoBehaviour
{
    [SerializeField]
    private Building MyBuilding;

    [SerializeField]
    public bool isHovered, isActive, isCoroutineFinished, Faded;

    Renderer[] renderers;

    public void Init(Building building)
    {
        this.MyBuilding = building;
        isHovered = false;
        isActive = false;
        Faded = false;
        isCoroutineFinished = true;
    }

    private void Start()
    {
        renderers = gameObject.GetComponentsInChildren<Renderer>();
        Debug.Log(renderers.Length);
    }

    private void Update()
    {
        if (!isActive)
        {
            if (isCoroutineFinished)
            {
                if (isHovered && !Faded)
                {
                    StartCoroutine(PreparePreview());
                    Debug.Log("Coroutine started PreparePreview");
                }
                else if (!isHovered && Faded)
                {
                    StartCoroutine(ResetPreview());
                    Debug.Log("Coroutine started ResetPreview");
                }
            }
        }
    }

    private IEnumerator PreparePreview()
    {
        isCoroutineFinished = false;
        List<Material> allMaterials = new List<Material>();
        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.materials;

            foreach (var mat in materials)
            {
                if (!mat.HasProperty("_Color")) continue;

                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.SetInt("_Surface", 1);
                mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

                mat.SetShaderPassEnabled("DepthOnly", false);
                mat.SetShaderPassEnabled("SHADOWCASTER", enabled);
                mat.SetOverrideTag("RenderType", "Transparent");

                mat.EnableKeyword("SURFACE_TYPE_TRANSPARENT");
                mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");

                allMaterials.Add(mat);
            }
        }


        float time = 0f;
        while (allMaterials.Count > 0 && allMaterials[0].color.a > 0.5f)
        {
            foreach (var mat in allMaterials)
            {
                Color c = mat.color;
                c.a = Mathf.Lerp(1f, 0.5f, time * 5f);
                mat.color = c;
            }

            time += Time.deltaTime;
            yield return null;
        }

        Faded = true;
        isCoroutineFinished = true;
    }


    private IEnumerator ResetPreview()
    {
        isCoroutineFinished = false;
        List<Material> allMaterials = new List<Material>();

        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.materials;

            foreach (var mat in materials)
            {
                if (!mat.HasProperty("_Color")) continue;
                allMaterials.Add(mat);
            }
        }

        float time = 0f;
        while (allMaterials.Count > 0 && allMaterials[0].color.a < 1.0f)
        {
            foreach (var mat in allMaterials)
            {
                Color c = mat.color;
                c.a = Mathf.Lerp(0.5f, 1.0f, time * 5f);
                mat.color = c;
            }

            time += Time.deltaTime;
            yield return null;
        }

        foreach (var mat in allMaterials)
        {
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            mat.SetInt("_ZWrite", 1);
            mat.SetInt("_Surface", 0);

            mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;
            mat.SetShaderPassEnabled("DepthOnly", true);
            mat.SetShaderPassEnabled("SHADOWCASTER", true);

            mat.SetOverrideTag("RenderType", "Opaque");

            mat.DisableKeyword("SURFACE_TYPE_TRANSPARENT");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        }
        Faded = false;
        isCoroutineFinished = true;
    }

    public void ShowUI(Material ActiveMaterial)
    {
        foreach (var ren in renderers)
        {
            Material[] materials = ren.materials;

            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = ActiveMaterial;
            }
        }
    }

    internal void HideUI()
    {
        //throw new NotImplementedException();
    }
}

//foreach (Renderer renderer in renderers)
//{
//    Material[] materials = renderer.materials;
//    foreach (var mat in materials)
//    {
//        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
//        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
//        mat.SetInt("_ZWrite", 0);
//        mat.SetInt("_Surface", 1);

//        mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
//        mat.SetShaderPassEnabled("DepthOnly", false);
//        mat.SetShaderPassEnabled("SHADOWCASTER", enabled);

//        mat.SetOverrideTag("RenderType", "Transparent");

//        mat.EnableKeyword("SURFACE_TYPE_TRANSPARENT");
//        mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
//    }

//    float time = 0f;
//    while (materials[0].color.a > 0.5f)
//    {
//        foreach (var mat in materials)
//        {
//            if (mat.HasProperty("_Color"))
//            {
//                mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, Mathf.Lerp(1f, 0.5f, time * 5f));
//            }
//        }

//        time += Time.deltaTime;
//        renderer.materials = materials;
//        yield return null;
//    }
//}