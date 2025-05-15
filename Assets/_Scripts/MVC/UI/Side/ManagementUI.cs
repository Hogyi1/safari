using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    public void OnValueChanged(int value)
    {
        ticketText.text = ("$" + value.ToString() + ",00");
    }

    public void OnRangersChanged(int value, int maxValue)
    {
        rangerSliderText.text = (value.ToString() + " / " + maxValue.ToString());
        rangerSlider.fillAmount = value / (float)maxValue;
    }

    public void OnAnimalChanged(int all, int herbivore, int carnivore)
    {
        animalCount.text = all.ToString("N0");
        herbivoreCount.text = herbivore.ToString("N0");
        carnivoreCount.text = carnivore.ToString("N0");
    }

    public void OnEconomyChanged(int income, int expenses, int rangerSalary)
    {
        allTimeIncome.text = "$ " + income.ToString("N0");
        allTimeOutcome.text = "$ " + expenses.ToString("N0");
        rangerSalaryText.text = "$ " + rangerSalary.ToString("N0");
    }
}
