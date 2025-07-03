using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ServiceManager : MonoBehaviour
{
    public TMP_Text RangerCount;
    public TMP_Text Salary;
    public Button HireButton;
    public Button FireButton;
    

    private void LateUpdate()
    {
        RangerCount.text = RangerManager.Instance.Capacity.ToString();
        Salary.text =  (EconomyManager.Instance.GetEconomy().RangerSalary * RangerManager.Instance.Capacity).ToString();

    }

}
