using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Manages the game's economy: tracks money, expenses, income, and handles transactions.
/// Implements a singleton pattern for global access.
/// </summary>
public class EconomyManager : MonoBehaviour, IDataPersistence
{
    /// <summary>
    /// Singleton instance of the EconomyManager.
    /// </summary>
    public static EconomyManager Instance { get; private set; }

    public float Priority => 0f;

    /// <summary>
    /// The underlying economy data object.
    /// </summary>
    private Economy Economy = new Economy();

    [SerializeField] private int maxTicketPrice = 50;

    public bool Incoming;
    private Action OnHandlerResponse;

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
        OnHandlerResponse = () => gameObject.SetActive(true);
        DataPersistenceManager.Instance.OnAllLoaded += OnHandlerResponse;

        gameObject.SetActive(false);
    }

    private void OnDestroy() => DataPersistenceManager.Instance.OnAllLoaded -= OnHandlerResponse;

    /// <summary>
    /// Initializes the economy data on start.
    /// </summary>
    private void Start()
    {
        if (Economy.IsUnityNull())
            Economy = new Economy();
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
            Incoming = false;
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
        GameEvents.Instance.NotifyObservers(EventType.MONEY_GAIN, amount);
        Incoming = true;
    }

    /// <summary>
    /// Calculates current expenses based on the number of rangers and their salary.
    /// </summary>
    public void CalculateExpenses()
    {
        int Rangers = RangerManager.Instance.Capacity;
        Economy.CurrentExpenses = Rangers * Economy.RangerSalary;
    }

    /// <summary>
    /// Processes a ticket purchase, adding ticket revenue to income.
    /// </summary>
    public void PayForTicket()
    {
        AddMoney(Economy.TicketPrice);
        Economy.OverallIncome += Economy.TicketPrice;
    }

    /// <summary>
    /// Pays salary for a ranger by removing funds equal to RangerSalary.
    /// </summary>
    /// <returns>True if the payment succeeded; otherwise false.</returns>
    public bool PaySalary()
    {
        CalculateExpenses();
        return RemoveMoney(Economy.RangerSalary);
    }

    /// <summary>
    /// Changes the ticket price within allowed bounds (1 to 50).
    /// </summary>
    /// <param name="price">The new ticket price.</param>
    public void ChangeTicketPrice(int price)
    {
        if (price > maxTicketPrice || price < 1) return;
        Economy.TicketPrice = price;
    }

    /// <summary>
    /// Checks whether there is enough money to cover a specified price.
    /// </summary>
    /// <param name="price">The price to check against current money.</param>
    /// <returns>True if current money is greater than or equal to the price; otherwise false.</returns>
    public bool HasEnoughMoney(int price) => Economy.CurrentMoney >= price;

    /// <summary>
    /// Retrieves the current Economy data instance.
    /// </summary>
    /// <returns>The underlying Economy object.</returns>
    public Economy GetEconomy() => Economy;

    public IEnumerator LoadData(GameData data)
    {
        Economy = data.Economy;
        yield return null;
    }

    public void SaveData(GameData data)
    {
        data.Economy = this.Economy;
    }


    /// <summary>
    /// The influence of the ticket price regarding the tourist spawning
    /// </summary>
    /// <returns>Ticket influence</returns>
    public float GetTicketInfluence() => Mathf.Clamp01(1f - Economy.TicketPrice / maxTicketPrice) + 0.5f;

}
