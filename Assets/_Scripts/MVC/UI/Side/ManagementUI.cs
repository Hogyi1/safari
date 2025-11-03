using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the visual updates of the management UI, displaying ticket prices, ranger stats, animal counts, and economy data.
/// </summary>
public class ManagementUI : MonoBehaviour
{
    // Ticket
    [SerializeField] private TextMeshProUGUI ticketText;

    // Rangers
    [SerializeField] private TextMeshProUGUI rangerSliderText;
    [SerializeField] private Image rangerSlider;
    [SerializeField] private TextMeshProUGUI rangerSalaryText;

    // Animals
    [SerializeField] private TextMeshProUGUI animalCount;
    [SerializeField] private TextMeshProUGUI herbivoreCount;
    [SerializeField] private TextMeshProUGUI carnivoreCount;

    // Economy
    [SerializeField] private TextMeshProUGUI allTimeIncome;
    [SerializeField] private TextMeshProUGUI allTimeOutcome;

    /// <summary>
    /// Updates the displayed ticket price value.
    /// </summary>
    /// <param name="value">The new ticket price.</param>
    public void OnValueChanged(int value)
    {
        ticketText.text = "$" + value.ToString() + ",00";
    }

    /// <summary>
    /// Updates the ranger capacity display, including the fill bar and label text.
    /// </summary>
    /// <param name="value">The current number of active rangers.</param>
    /// <param name="maxValue">The maximum ranger capacity.</param>
    public void OnRangersChanged(int value, int maxValue)
    {
        rangerSliderText.text = value.ToString() + " / " + maxValue.ToString();
        rangerSlider.fillAmount = value / (float)maxValue;
    }

    /// <summary>
    /// Updates the displayed total animal count, herbivore count, and carnivore count.
    /// </summary>
    /// <param name="all">Total number of animals.</param>
    /// <param name="herbivore">Number of herbivores.</param>
    /// <param name="carnivore">Number of carnivores.</param>
    public void OnAnimalChanged(int all, int herbivore, int carnivore)
    {
        animalCount.text = all.ToString("N0");
        herbivoreCount.text = herbivore.ToString("N0");
        carnivoreCount.text = carnivore.ToString("N0");
    }

    /// <summary>
    /// Updates the economy UI with current income, expenses, and ranger salary.
    /// </summary>
    /// <param name="income">Total income amount.</param>
    /// <param name="expenses">Total expenses amount.</param>
    /// <param name="rangerSalary">Total ranger salary amount.</param>
    public void OnEconomyChanged(int income, int expenses, int rangerSalary)
    {
        allTimeIncome.text = "$ " + income.ToString("N0");
        allTimeOutcome.text = "$ " + expenses.ToString("N0");
        rangerSalaryText.text = "$ " + rangerSalary.ToString("N0");
    }
}
