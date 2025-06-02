using UnityEngine;

public class DiurnalCycleView : MonoBehaviour
{
    #region References

    /// <summary>
    /// The skybox material that uses the “Custom/SkyboxBlend” shader. 
    /// The material should expose cubemaps for day/night and a _Blend property.
    /// </summary>
    [Header("Skybox Settings")]
    [Tooltip("Material using the \"Custom/SkyboxBlend\" shader (with _CubemapDay, _CubemapNight, _Blend).")]
    public Material skyboxMaterial;

    /// <summary>
    /// The directional light (sun) in the scene whose intensity will be controlled.
    /// </summary>
    [Header("Global Light Settings")]
    [Tooltip("The scene’s directional (sun) light.")]
    public Light directionalLight;

    /// <summary>
    /// The light intensity used during full day (when blend == 0).
    /// </summary>
    [Tooltip("Intensity at full day (blend = 0).")]
    public float dayLightIntensity = 1f;

    /// <summary>
    /// The light intensity used during full night (when blend == 1).
    /// </summary>
    [Tooltip("Intensity at full night (blend = 1).")]
    public float nightLightIntensity = 0f;

    /// <summary>
    /// The speed at which the current blend value moves toward the target blend (units per second).
    /// </summary>
    [Header("Fade Settings")]
    [Tooltip("How many blend‐units per second to move (0 to 1). " +
             "E.g. 1 = takes 1 second to fade fully; 0.5 = 2 seconds, etc.")]
    public float blendSpeed = 1f;

    // The “live” blend value that is currently applied (ranges 0 to 1).
    private float currentBlend = 0f;

    // What the controller most‐recently set as the desired blend. 
    // The Update() loop will move currentBlend toward this value each frame.
    private float targetBlend = 0f;

    #endregion

    /// <summary>
    /// Called on the frame when the script is enabled. Assigns the skybox material and initializes blend values.
    /// </summary>
    private void Start()
    {
        // Immediately assign our blended skybox so we can begin fading.
        if (skyboxMaterial != null)
        {
            RenderSettings.skybox = skyboxMaterial;

            // Force global illumination update so ambient and reflection probes match the new sky.
            DynamicGI.UpdateEnvironment();
        }
        else
        {
            Debug.LogWarning("[DiurnalCycleView] No skyboxMaterial assigned! Skybox will not update.");
        }

        // Initialize currentBlend based on any value manually set in the Editor (if applicable).
        if (skyboxMaterial != null)
        {
            currentBlend = skyboxMaterial.GetFloat("_Blend");
            targetBlend = currentBlend;
        }
    }

    /// <summary>
    /// Called every frame to interpolate the blend value toward the target and apply it to the skybox and directional light.
    /// </summary>
    private void Update()
    {
        if (!Mathf.Approximately(currentBlend, targetBlend))
        {
            currentBlend = Mathf.MoveTowards(currentBlend, targetBlend, blendSpeed * Time.deltaTime);

            ApplyBlend(currentBlend);
        }
    }

    /// <summary>
    /// Sets the desired blend value for the day/night transition.
    /// </summary>
    /// <param name="b">Blend value between 0 (full day) and 1 (full night). This value will be clamped to [0,1].</param>
    public void SetTargetBlend(float b)
    {
        targetBlend = Mathf.Clamp01(b);
    }

    /// <summary>
    /// Applies the given blend value to the skybox material (_Blend property) and adjusts the directional light intensity accordingly.
    /// </summary>
    /// <param name="blend">Blend value between 0 (full day) and 1 (full night).</param>
    private void ApplyBlend(float blend)
    {
        if (skyboxMaterial != null)
        {
            skyboxMaterial.SetFloat("_Blend", blend);

            // Force global illumination update so reflection probes and ambient lighting update smoothly.
            DynamicGI.UpdateEnvironment();
        }

        if (directionalLight != null)
        {
            directionalLight.intensity = Mathf.Lerp(dayLightIntensity, nightLightIntensity, blend);
        }
    }
}
