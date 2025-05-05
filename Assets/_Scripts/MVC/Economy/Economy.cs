using UnityEngine;

public class Economy
{
    // Adattagok
    public int CurrentMoney;

    public int CurrentExpenses;

    public int TicketPrice;

    public int RangerSalary;

    public int OverallIncome = 0;

    public int OverallExpense = 0;

    public Economy()
    {
        CurrentMoney = 500;
        CurrentExpenses = 0;
        TicketPrice = 10;
        RangerSalary = 100;
    }

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
