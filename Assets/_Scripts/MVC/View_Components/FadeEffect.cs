using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FadeEffect : MonoBehaviour
{
    private Renderer[] renderers;
    private Coroutine fadeRoutine;

    [SerializeField] private float speed = 5f, fadeAmount = 0.65f;
    public bool IsFaded { get; private set; } = false;
    [SerializeField] private bool HasShader = false;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
    }

    public void FadeOut()
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(ResetView());
        IsFaded = false;
    }

    public void FadeIn()
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeView());
        IsFaded = true;
    }

    private IEnumerator FadeView()
    {
        List<Material> allMaterials = new List<Material>();
        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.materials;

            foreach (var mat in materials)
            {
                if (mat.HasProperty("_NotFadeable")) continue;

                if (!HasShader)
                {
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
                }

                allMaterials.Add(mat);
            }
        }

        float time = 0f;
        if (!HasShader)
        {
            float currentFade = allMaterials[0].color.a;

            while (allMaterials.Count > 0 && allMaterials[0].color.a > fadeAmount)
            {
                foreach (var mat in allMaterials)
                {
                    Color c = mat.color;
                    c.a = Mathf.Lerp(currentFade, fadeAmount, time * speed);
                    mat.color = c;
                }

                time += Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            float Current = allMaterials[0].GetFloat("_AlphaClip");

            while (allMaterials[0].GetFloat("_AlphaClip") > fadeAmount)
            {
                foreach (var mat in allMaterials)
                {
                    float To = Mathf.Lerp(Current, fadeAmount, time * speed);
                    mat.SetFloat("_AlphaClip", To);
                }

                time += Time.deltaTime;
                yield return null;
            }
        }
    }


    private IEnumerator ResetView()
    {
        List<Material> allMaterials = new List<Material>();

        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.materials;

            foreach (var mat in materials)
            {
                allMaterials.Add(mat);
            }
        }

        float time = 0f;
        if (!HasShader)
        {
            float currentFade = allMaterials[0].color.a;

            while (allMaterials.Count > 0 && allMaterials[0].color.a < 1.0f)
            {
                foreach (var mat in allMaterials)
                {
                    Color c = mat.color;
                    c.a = Mathf.Lerp(currentFade, 1.0f, time * speed);
                    mat.color = c;
                }

                time += Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            float Current = allMaterials[0].GetFloat("_AlphaClip");

            while (allMaterials[0].GetFloat("_AlphaClip") < 1.0f)
            {
                foreach (var mat in allMaterials)
                {
                    float To = Mathf.Lerp(Current, 1.0f, time * speed);
                    mat.SetFloat("_AlphaClip", To);
                }

                time += Time.deltaTime;
                yield return null;
            }
        }

        if (!HasShader)
        {
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

}
