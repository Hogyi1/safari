using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the statistics display for animals, tourists, and economy,
/// including arrow direction and color feedback based on positive/negative changes.
/// </summary>
public class StatisticsUI : MonoBehaviour
{
    // === Animals ===

    [SerializeField] private TextMeshProUGUI animalText;
    [SerializeField] private Image animalArrow;
    [SerializeField] private Image animalCircle;

    // === Tourists ===

    [SerializeField] private TextMeshProUGUI touristText;
    [SerializeField] private Image touristArrow;
    [SerializeField] private Image touristCircle;

    // === Economy ===

    [SerializeField] private TextMeshProUGUI economyText;
    [SerializeField] private Image economyArrow;
    [SerializeField] private Image economyCircle;

    [Header("Colors")]
    [SerializeField] private Color badColor = Color.red;
    [SerializeField] private Color goodColor = Color.green;

    /// <summary>
    /// Updates the animal statistics: value display, arrow direction, and circle color.
    /// </summary>
    /// <param name="value">The numeric value to display.</param>
    /// <param name="positive">Whether the change is positive or negative.</param>
    public void UpdateAnimal(int value, bool positive)
    {
        animalText.text = value.ToString();
        animalArrow.rectTransform.localRotation = Quaternion.Euler(positive ? 0f : 180f, 0f, 0f);
        animalCircle.color = positive ? goodColor : badColor;
    }

    /// <summary>
    /// Updates the tourist statistics: value display, arrow direction, and circle color.
    /// </summary>
    /// <param name="value">The numeric value to display.</param>
    /// <param name="positive">Whether the change is positive or negative.</param>
    public void UpdateTourist(int value, bool positive)
    {
        touristText.text = value.ToString();
        touristArrow.rectTransform.localRotation = Quaternion.Euler(positive ? 0f : 180f, 0f, 0f);
        touristCircle.color = positive ? goodColor : badColor;
    }

    /// <summary>
    /// Updates the economy statistics: value display, arrow direction, and circle color.
    /// </summary>
    /// <param name="value">The numeric value to display.</param>
    /// <param name="positive">Whether the change is positive or negative.</param>
    public void UpdateEconomy(int value, bool positive)
    {
        economyText.text = value.ToString("N0");
        economyArrow.rectTransform.localRotation = Quaternion.Euler(positive ? 0f : 180f, 0f, 0f);
        economyCircle.color = positive ? goodColor : badColor;
    }
}
