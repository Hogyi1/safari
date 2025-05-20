using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the visual display of tourist-related data.
/// </summary>
public class TouristUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image tourMood;
    [SerializeField] private Image tourIcon;
    [SerializeField] private Image waitingMood;
    [SerializeField] private Image waitingIcon;
    [SerializeField] private Image ticketMood;
    [SerializeField] private Image ticketIcon;
    [SerializeField] private TextMeshProUGUI favAnimal;
    [SerializeField] private TextMeshProUGUI allTimeTouristCount;

    [Header("Mood Colors")]
    [SerializeField] private Color badColor = Color.red;
    [SerializeField] private Color neutralColor = new Color(1f, 0.64f, 0f);
    [SerializeField] private Color goodColor = Color.green;

    /// <summary>
    /// Updates the favorite animal text in the UI.
    /// </summary>
    /// <param name="animal">The name of the favorite animal.</param>
    public void SetFavoriteAnimal(string animal)
    {
        favAnimal.text = $"Favourite animal: {animal}";
    }

    /// <summary>
    /// Updates the total tourist count text in the UI.
    /// </summary>
    /// <param name="count">The total number of tourists to display.</param>
    public void SetTouristCount(int count)
    {
        allTimeTouristCount.text = $"All time visitors: {count:N0}";
    }

    /// <summary>
    /// Updates a specific mood bar and icon color based on the mood value.
    /// Mood is interpolated between bad, neutral, and good colors.
    /// </summary>
    /// <param name="moodBar">The Image component used as the mood bar.</param>
    /// <param name="icon">The associated mood icon.</param>
    /// <param name="moodValue">The mood percentage (0–100).</param>
    public void SetMood(Image moodBar, Image icon, float moodValue)
    {
        float t = Mathf.Clamp01(moodValue / 100f);
        Color moodColor;

        if (t < 0.5f)
            moodColor = Color.Lerp(badColor, neutralColor, t / 0.5f);
        else
            moodColor = Color.Lerp(neutralColor, goodColor, (t - 0.5f) / 0.5f);

        moodBar.fillAmount = t;
        moodBar.color = moodColor;
        if (icon != null)
            icon.color = moodColor;
    }

    /// <summary>
    /// Updates all mood bars and icons (tour experience, waiting time, and ticket pricing).
    /// </summary>
    /// <param name="overallMood">Mood for the tour experience.</param>
    /// <param name="waitingMoodValue">Mood for waiting times.</param>
    /// <param name="feeMood">Mood for ticket pricing.</param>
    public void SetAllMoods(float overallMood, float waitingMoodValue, float feeMood)
    {
        SetMood(tourMood, tourIcon, overallMood);
        SetMood(waitingMood, waitingIcon, waitingMoodValue);
        SetMood(ticketMood, ticketIcon, feeMood);
    }
}
