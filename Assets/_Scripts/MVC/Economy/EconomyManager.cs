    using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    // Singleton pattern
    public static EconomyManager Instance { get; private set; }

    // Az economy amire mindenki lálát
    private Economy Economy { get; set; }

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        this.Economy = new Economy();
    }

    // Methods
    public bool RemoveMoney(int amount)
    {
        if (HasEnoughMoney(amount))
        {
            Economy.CurrentMoney -= amount;
            Economy.OverallExpense += amount;
            return true;
        }
        return false;
    }

    public void AddMoney(int amount)
    {
        Economy.CurrentMoney += amount;
        Economy.OverallIncome += amount;
    }

    public void CalculateExpenses()
    {
        var Rangers = 0; // NPCManager.GetRangers().Count();
        Economy.CurrentExpenses = Rangers * Economy.RangerSalary;
    }

    public void PayForTicket()
    {
        AddMoney(Economy.TicketPrice);
        // Economy.OverallIncome += Economy.TicketPrice;
    }

    public bool PaySalary()
    {
        return RemoveMoney(Economy.RangerSalary);
    }

    public void ChangeTicketPrice(int price)
    {
        if (price > 50 || price < 1)
        {
            return;
        }

        Economy.TicketPrice = price;
    }

    public bool HasEnoughMoney(int price)
    {
        return Economy.CurrentMoney >= price;
    }

    public int GetSellingPrice(int price)
    {
        return (int)Mathf.Round(price * 0.5f);
    }

    public Economy getEconomy() {

        return this.Economy;
    
    }


}
