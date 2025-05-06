using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages opacity transitions for materials containing 'leaves' and optional particle effects.
/// Collects relevant materials on start and provides a method to interpolate their shader '_Amount' property.
/// </summary>
public class StageEffect : MonoBehaviour
{
    /// <summary>
    /// List of materials to modify during opacity transitions (e.g., leaf shaders).
    /// </summary>
    [SerializeField] private List<Material> changeableMaterials = new List<Material>();

    /// <summary>
    /// Speed multiplier determining how fast the opacity transitions occur.
    /// </summary>
    [SerializeField] private float speed = 3.0f;

    /// <summary>
    /// Optional ParticleSystem to play or stop based on opacity threshold.
    /// Currently unused as particle logic is commented out.
    /// </summary>
    private ParticleSystem particles;

    /// <summary>
    /// Initializes the particle system (if present) and populates changeableMaterials
    /// with any materials whose names contain 'leaves' (case-insensitive).
    /// </summary>
    private void Start()
    {
        if (particles = gameObject.GetComponentInChildren<ParticleSystem>())
        {
            particles.gameObject.SetActive(false);
        }

        Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            foreach (Material mat in renderer.materials)
            {
                if (mat != null && mat.name.ToLower().Contains("leaves"))
                {
                    changeableMaterials.Add(mat);
                }
            }
        }
    }

    /// <summary>
    /// Starts a coroutine to change the materials' '_Amount' property to the specified target amount.
    /// </summary>
    /// <param name="targetAmount">The desired opacity or shader amount to reach.</param>
    public void SetMaterialsOpacity(float targetAmount)
    {
        StartCoroutine(ChangeMaterials(targetAmount));
    }

    /// <summary>
    /// Coroutine that linearly interpolates each material's '_Amount' property from its current value
    /// to the target value over time, using the configured speed.
    /// </summary>
    /// <param name="to">Target shader '_Amount' value.</param>
    /// <returns>IEnumerator for coroutine execution.</returns>
    private IEnumerator ChangeMaterials(float to)
    {
        float time = 0f;
        float from = changeableMaterials[0].GetFloat("_Amount");
        while (Mathf.Abs(changeableMaterials[0].GetFloat("_Amount") - to) > 0.01f)
        {
            foreach (var mat in changeableMaterials)
            {
                float To = Mathf.Lerp(from, to, time * speed);
                mat.SetFloat("_Amount", To);
            }

            time += Time.deltaTime;
            yield return null;
        }

        foreach (var mat in changeableMaterials)
        {
            mat.SetFloat("_Amount", to);
        }

    }
}
