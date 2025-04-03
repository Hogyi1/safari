using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeEffect : MonoBehaviour
{
    private Renderer[] renderers;
    private Coroutine fadeRoutine;

    [SerializeField]
    [Tooltip("Minél kisebb annál több idő")]
    private float duration = 3f;
    private float fadeAmount = 0.5f;
    public bool IsFaded { get; private set; } = false;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
    }

    public void FadeOut()
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(Reset());
        IsFaded = false;
    }

    public void FadeIn()
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(Hovered());
        IsFaded = true;
    }

    private IEnumerator Hovered()
    {
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
        while (allMaterials.Count > 0 && allMaterials[0].color.a > fadeAmount)
        {
            foreach (var mat in allMaterials)
            {
                Color c = mat.color;
                c.a = Mathf.Lerp(1f, 0.5f, time * duration);
                mat.color = c;
            }

            time += Time.deltaTime;
            yield return null;
        }
    }


    private IEnumerator Reset()
    {
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
                c.a = Mathf.Lerp(0.5f, 1.0f, time * duration);
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
    }

}
