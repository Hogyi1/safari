using UnityEngine;

/// <summary>
/// Stores economy data such as current money, expenses, ticket price, and ranger salary.
/// </summary>
public class Economy
{
    /// <summary>
    /// The player's current money balance.
    /// </summary>
    public int CurrentMoney = 0;

    /// <summary>
    /// The player's current calculated expenses.
    /// </summary>
    public int CurrentExpenses = 0;

    /// <summary>
    /// The price charged per ticket.
    /// </summary>
    public int TicketPrice = 0;

    /// <summary>
    /// The salary paid to each ranger.
    /// </summary>
    public int RangerSalary = 0;

    /// <summary>
    /// The total income accumulated since the start.
    /// </summary>
    public int OverallIncome = 0;

    /// <summary>
    /// The total expenses accumulated since the start.
    /// </summary>
    public int OverallExpense = 0;

    /// <summary>
    /// Initializes default economy values.
    /// </summary>
    public Economy()
    {
        CurrentMoney = 2500;
        CurrentExpenses = 0;
        TicketPrice = 10;
        RangerSalary = 100;
    }

    /// <summary>
    /// Initializes economy values with custom parameters.
    /// </summary>
    /// <param name="currentMoney">Initial current money balance.</param>
    /// <param name="currentExpenses">Initial current expenses.</param>
    /// <param name="ticketPrice">Initial ticket price.</param>
    /// <param name="rangerSalary">Initial ranger salary.</param>
    /// <param name="overallIncome">Initial overall income.</param>
    /// <param name="overallExpense">Initial overall expense.</param>
    public Economy(int currentMoney, int currentExpenses, int ticketPrice, int rangerSalary, int overallIncome, int overallExpense)
    {
        CurrentMoney = currentMoney;
        CurrentExpenses = currentExpenses;
        TicketPrice = ticketPrice;
        RangerSalary = rangerSalary;
        OverallIncome = overallIncome;
        OverallExpense = overallExpense;
    }
}
