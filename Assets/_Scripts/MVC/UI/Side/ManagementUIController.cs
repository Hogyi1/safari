using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the Management UI by fetching data from managers and passing it to the view.
/// </summary>
public class ManagementUIController : MonoBehaviour
{
    [SerializeField] private ManagementUI ui;
    [SerializeField] private Slider ticketSlider;

    private void Start()
    {
        ticketSlider.value = 10;//EconomyManager.Instance.GetEconomy().TicketPrice;
    }

    private void LateUpdate()
    {
        UpdateEconomy();
        UpdateAnimals();
        UpdateRangers();
    }

    public void OnSliderMove()
    {
        int value = Mathf.RoundToInt(ticketSlider.value);
        EconomyManager.Instance.ChangeTicketPrice(value);
        ui.OnValueChanged(value);
    }

    private void UpdateEconomy()
    {
        Economy economy = EconomyManager.Instance.GetEconomy();
        ui.OnEconomyChanged(economy.OverallIncome, economy.OverallExpense, economy.RangerSalary);
    }

    private void UpdateAnimals()
    {
        var animalMgr = AnimalManager.Instance;
        ui.OnAnimalChanged(animalMgr.Count, animalMgr.HerbivoreCount, animalMgr.CarnivoreCount);
    }

    private void UpdateRangers()
    {
        var rangerMgr = RangerManager.Instance;
        ui.OnRangersChanged(rangerMgr.Capacity, rangerMgr.MaxCapacity);
    }

}
