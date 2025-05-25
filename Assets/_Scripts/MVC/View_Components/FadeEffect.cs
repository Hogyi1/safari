using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles fade-in and fade-out effects for all child renderers of a GameObject,
/// either by adjusting material color alpha or a shader's '_AlphaClip' property.
/// </summary>
public class FadeEffect : MonoBehaviour
{
    /// <summary>
    /// Cached array of all Renderer components in children to apply fade effect.
    /// </summary>
    private Renderer[] renderers;

    /// <summary>
    /// Reference to the currently running fade coroutine.
    /// </summary>
    private Coroutine fadeRoutine;

    /// <summary>
    /// Speed multiplier controlling how fast the fade occurs.
    /// </summary>
    [SerializeField] private float speed = 5f;

    /// <summary>
    /// Target alpha or clip value to fade to (0 to 1).
    /// </summary>
    [SerializeField] private float fadeAmount = 0.65f;

    /// <summary>
    /// Indicates whether the object is currently faded.
    /// </summary>
    public bool IsFaded { get; private set; } = false;

    /// <summary>
    /// When true, uses a shader's '_AlphaClip' property instead of material color alpha.
    /// </summary>
    [SerializeField] private bool HasShader = false;

    /// <summary>
    /// Caches all child renderers on awake.
    /// </summary>
    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
    }

    /// <summary>
    /// Fades the object back to full opacity by running the ResetView coroutine.
    /// </summary>
    public void FadeOut()
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(ResetView());
        IsFaded = false;
    }

    /// <summary>
    /// Fades the object to the configured fadeAmount by running the FadeView coroutine.
    /// </summary>
    public void FadeIn()
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeView());
        IsFaded = true;
    }

    /// <summary>
    /// Coroutine that reduces materials' transparency or shader clip value to fadeAmount.
    /// Configures blend settings for transparency when HasShader is false.
    /// </summary>
    /// <returns>IEnumerator for coroutine execution.</returns>
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
                    // Configure material for transparent blending
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

                time += Time.unscaledDeltaTime;
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

                time += Time.unscaledDeltaTime;
                yield return null;
            }
        }
    }

    /// <summary>
    /// Coroutine that restores materials to full opacity and resets blend settings if HasShader is false.
    /// </summary>
    /// <returns>IEnumerator for coroutine execution.</returns>
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
