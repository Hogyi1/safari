using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the Management UI by fetching data from managers and passing it to the view.
/// </summary>
public class ManagementUIController : MonoBehaviour
{
    [SerializeField] private ManagementUI ui;
    [SerializeField] private Slider ticketSlider;

    /// <summary>
    /// Initializes the ticket price slider with a default value.
    /// </summary>
    private void Start()
    {
        ticketSlider.value = 10; // EconomyManager.Instance.GetEconomy().TicketPrice;
    }

    /// <summary>
    /// Called every frame after all Update methods. Updates UI with the latest game data.
    /// </summary>
    private void LateUpdate()
    {
        UpdateEconomy();
        UpdateAnimals();
        UpdateRangers();
    }

    /// <summary>
    /// Called when the ticket price slider is moved. Updates the ticket price in the economy manager and informs the UI.
    /// </summary>
    public void OnSliderMove()
    {
        int value = Mathf.RoundToInt(ticketSlider.value);
        EconomyManager.Instance.ChangeTicketPrice(value);
        ui.OnValueChanged(value);
    }

    /// <summary>
    /// Retrieves economy data from the EconomyManager and updates the UI.
    /// </summary>
    private void UpdateEconomy()
    {
        Economy economy = EconomyManager.Instance.GetEconomy();
        ui.OnEconomyChanged(economy.OverallIncome, economy.OverallExpense, economy.RangerSalary);
    }

    /// <summary>
    /// Retrieves animal statistics from the AnimalManager and updates the UI.
    /// </summary>
    private void UpdateAnimals()
    {
        var animalMgr = AnimalManager.Instance;
        ui.OnAnimalChanged(animalMgr.Count, animalMgr.HerbivoreCount, animalMgr.CarnivoreCount);
    }

    /// <summary>
    /// Retrieves ranger data from the RangerManager and updates the UI.
    /// </summary>
    private void UpdateRangers()
    {
        var rangerMgr = RangerManager.Instance;
        ui.OnRangersChanged(rangerMgr.Capacity, rangerMgr.MaxCapacity);
    }
}
