using UnityEngine;

public class Economy
{
    // Adattagok
    [SerializeField]
    public int CurrentMoney = 500;
    [SerializeField]
    public int CurrentExpenses = 0;
    [SerializeField]
    [Range(1, 50)]
    public int TicketPrice = 10;
    [SerializeField]
    public int RangerSalary = 1000;
    [SerializeField]
    public int OverallIncome = 0;
    [SerializeField]
    public int OverallExpense = 0;
}
