using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static UIComponent;
public class ButtonComponent : MonoBehaviour, IUIComponent
{
    [SerializeField] private Button refill, upgrade;
    [SerializeField] private TextMeshProUGUI RefillText;
    [SerializeField] private UIComponent refillKey = Refillprice_button;
    [SerializeField] private UIComponent upgradeKey = Upgradeprice_button;
    private int price;

    // Csakis a BuildingUI-hoz fog működni
    public void TrySetup(Dictionary<UIComponent, object> data)
    {
        if (data.TryGetValue(refillKey, out var r))
        {
            price = (int)r;
            RefillText.text = "Refill $" + r.ToString();
            refill.gameObject.SetActive(true);
            refill.onClick.RemoveAllListeners();
            refill.onClick.AddListener(() => FeederManager.Instance.Refill((int)data[ID], price));
            return;
        }
        else { refill.gameObject.SetActive(false); RefillText.gameObject.SetActive(false); }

        if (data.TryGetValue(upgradeKey, out var u))
        {
            price = (int)u;
            upgrade.gameObject.SetActive(true);
        }
        else { upgrade.gameObject.SetActive(false); }
    }

    void Update()
    {
        if (refill != null)
        {
            refill.enabled = EconomyManager.Instance.HasEnoughMoney(price);
        }

        if (upgrade != null)
        {
            upgrade.enabled = EconomyManager.Instance.HasEnoughMoney(price);
        }
    }
}

