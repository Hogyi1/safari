using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the UI display for tourist-related data including:
/// - Overall mood values with color transitions (bad → neutral → good)
/// - Favorite animal display (text)
/// - Total number of tourists (formatted number)
/// </summary>
public class TouristUI : MonoBehaviour
{
    // Serialized UI references
    [SerializeField] private Image tourMood;       // Mood bar for the tour experience
    [SerializeField] private Image tourIcon;       // Icon representing tour mood
    [SerializeField] private Image waitingMood;    // Mood bar for waiting time
    [SerializeField] private Image waitingIcon;    // Icon representing waiting mood
    [SerializeField] private Image ticketMood;     // Mood bar for ticket pricing
    [SerializeField] private Image ticketIcon;     // Icon representing ticket mood
    [SerializeField] private TextMeshProUGUI favAnimal;            // Displays most popular favorite animal
    [SerializeField] private TextMeshProUGUI allTimeTouristCount;  // Displays total tourist count (all-time)

    [Header("Mood Colors")]
    [SerializeField] private Color badColor = Color.red;                    // Mood color for low mood (0%)
    [SerializeField] private Color neutralColor = new Color(1f, 0.64f, 0f); // Mood color for 50%
    [SerializeField] private Color goodColor = Color.green;                // Mood color for high mood (100%)

    /// <summary>
    /// Updates the UI each frame:
    /// - Sets mood bars and icons based on mood values (0–100)
    /// - Updates favorite animal and tourist count text
    /// </summary>
    private void LateUpdate()
    {
        favAnimal.text = "Favourite animal: " + TouristManager.Instance.FavouriteAnimal.ToString();

        allTimeTouristCount.text = "All time visitors: " +
            TouristManager.Instance.AllTimeVisitors.ToString("N0"); // Adds thousands separator

        UpdateMood(tourMood, tourIcon, TouristManager.Instance.OverallMood);
        UpdateMood(waitingMood, waitingIcon, TouristManager.Instance.OverallWaitingMood);
        UpdateMood(ticketMood, ticketIcon, TouristManager.Instance.OverallFeeMood);
    }

    /// <summary>
    /// Updates a mood bar and icon with fillAmount and interpolated color.
    /// Mood transitions:
    /// - 0% to 50%: badColor → neutralColor
    /// - 50% to 100%: neutralColor → goodColor
    /// </summary>
    private void UpdateMood(Image moodBar, Image icon, float moodValue)
    {
        float t = Mathf.Clamp01(moodValue / 100f);
        Color moodColor;

        if (t < 0.5f)
        {
            float blend = t / 0.5f;
            moodColor = Color.Lerp(badColor, neutralColor, blend);
        }
        else
        {
            float blend = (t - 0.5f) / 0.5f;
            moodColor = Color.Lerp(neutralColor, goodColor, blend);
        }

        moodBar.fillAmount = t;
        moodBar.color = moodColor;

        if (icon != null)
            icon.color = moodColor;
    }
}
