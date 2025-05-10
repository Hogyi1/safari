    using UnityEngine;

/// <summary>
/// Manages the game's economy: tracks money, expenses, income, and handles transactions.
/// Implements a singleton pattern for global access.
/// </summary>
public class EconomyManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the EconomyManager.
    /// </summary>
    public static EconomyManager Instance { get; private set; }

    /// <summary>
    /// The underlying economy data object.
    /// </summary>
    private Economy Economy { get; set; }

    /// <summary>
    /// Ensures only one instance exists and persists across scenes.
    /// </summary>
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

    /// <summary>
    /// Initializes the economy data on start.
    /// </summary>
    private void Start()
    {
        this.Economy = new Economy();
    }

    /// <summary>
    /// Attempts to remove a specified amount of money.
    /// </summary>
    /// <param name="amount">The amount to remove from current money.</param>
    /// <returns>True if the removal succeeded; otherwise false.</returns>
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

    /// <summary>
    /// Adds a specified amount of money to the economy.
    /// </summary>
    /// <param name="amount">The amount to add to current money.</param>
    public void AddMoney(int amount)
    {
        Economy.CurrentMoney += amount;
        Economy.OverallIncome += amount;
    }

    /// <summary>
    /// Calculates current expenses based on the number of rangers and their salary.
    /// </summary>
    public void CalculateExpenses()
    {
        var Rangers = 0; // NPCManager.GetRangers().Count();
        Economy.CurrentExpenses = Rangers * Economy.RangerSalary;
    }

    /// <summary>
    /// Processes a ticket purchase, adding ticket revenue to income.
    /// </summary>
    public void PayForTicket()
    {
        AddMoney(Economy.TicketPrice);
        // Economy.OverallIncome += Economy.TicketPrice;
    }

    /// <summary>
    /// Pays salary for a ranger by removing funds equal to RangerSalary.
    /// </summary>
    /// <returns>True if the payment succeeded; otherwise false.</returns>
    public bool PaySalary()
    {
        return RemoveMoney(Economy.RangerSalary);
    }

    /// <summary>
    /// Changes the ticket price within allowed bounds (1 to 50).
    /// </summary>
    /// <param name="price">The new ticket price.</param>
    public void ChangeTicketPrice(int price)
    {
        if (price > 50 || price < 1)
        {
            return;
        }

        Economy.TicketPrice = price;
    }

    /// <summary>
    /// Checks whether there is enough money to cover a specified price.
    /// </summary>
    /// <param name="price">The price to check against current money.</param>
    /// <returns>True if current money is greater than or equal to the price; otherwise false.</returns>
    public bool HasEnoughMoney(int price)
    {
        return Economy.CurrentMoney >= price;
    }

    /// <summary>
    /// Calculates the selling price for an item at 50% of its original price.
    /// </summary>
    /// <param name="price">The original price of the item.</param>
    /// <returns>The selling price rounded to the nearest integer.</returns>
    public int GetSellingPrice(int price)
    {
        return (int)Mathf.Round(price * 0.5f);
    }

    /// <summary>
    /// Retrieves the current Economy data instance.
    /// </summary>
    /// <returns>The underlying Economy object.</returns>
    public Economy getEconomy() {

        return this.Economy;
    
    }


}
