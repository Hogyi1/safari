using UnityEngine;

[RequireComponent(typeof(DiurnalCycleView))]
public class DiurnalCycleController : MonoBehaviour
{
    #region References

    [Header("Sunrise (night to day)")]
    [Tooltip("Hour when fade begins (blend = 1)")]
    [Range(0f, 23.99f)]
    /// <summary>
    /// The hour at which the sunrise fade begins (blend value = 1 at this time).
    /// </summary>
    public float sunriseStart = 5f;

    [Tooltip("Hour when fade ends (blend = 0)")]
    [Range(0f, 23.99f)]
    /// <summary>
    /// The hour at which the sunrise fade ends (blend value = 0 at this time).
    /// </summary>
    public float sunriseEnd = 6f;

    [Header("Sunset (day to night)")]
    [Tooltip("Hour when fade begins (blend = 0)")]
    [Range(0f, 23.99f)]
    /// <summary>
    /// The hour at which the sunset fade begins (blend value = 0 at this time).
    /// </summary>
    public float sunsetStart = 18f;

    [Tooltip("Hour when fade ends (blend = 1)")]
    [Range(0f, 23.99f)]
    /// <summary>
    /// The hour at which the sunset fade ends (blend value = 1 at this time).
    /// </summary>
    public float sunsetEnd = 19f;

    private DiurnalCycleView view;

    #endregion

    /// <summary>
    /// Called when the script instance is being loaded. Fetches the <see cref="DiurnalCycleView"/> component on the same GameObject.
    /// </summary>
    private void Awake()
    {
        view = GetComponent<DiurnalCycleView>();
        if (view == null)
        {
            Debug.LogError("[DiurnalCycleController] Missing DiurnalCycleView on the same GameObject.");
        }
    }

    /// <summary>
    /// Called every frame. Retrieves the current in‐game time from the <see cref="TimeManager"/>, computes the blend value based on sunrise/sunset settings, and instructs the view to update.
    /// </summary>
    private void Update()
    {
        if (TimeManager.Instance == null) return;

        GameTime gt = TimeManager.Instance.GetCurrentTime();

        float currentHour = gt.Hours + (gt.Minutes / 60f);
        float blend = CalculateBlend(currentHour);

        Debug.Log($"[DiurnalCycleController] Time is {gt.Hours:00}:{gt.Minutes:00}. Blend = {blend:0.00}");

        view.SetTargetBlend(blend);
    }

    /// <summary>
    /// Calculates the blend value for skybox and lighting based on the given hour of the day.
    /// </summary>
    /// <param name="hour">Current in‐game time expressed as a floating‐point hour (0 to 23.99).</param>
    /// <returns>
    /// A blend value between 0 (full day) and 1 (full night). 
    /// Returns intermediate values during sunrise (descending from 1 to 0) and sunset (ascending from 0 to 1).
    /// </returns>
    private float CalculateBlend(float hour)
    {
        // Sunrise fade (nigh to day)
        if (hour >= sunriseStart && hour <= sunriseEnd)
        {
            return 1f - ((hour - sunriseStart) / (sunriseEnd - sunriseStart));
        }
        // Full day
        if (hour > sunriseEnd && hour < sunsetStart)
        {
            return 0f;
        }
        // Sunset fade (day to night)
        if (hour >= sunsetStart && hour <= sunsetEnd)
        {
            return (hour - sunsetStart) / (sunsetEnd - sunsetStart);
        }
        // Full night
        return 1f;
    }
}
