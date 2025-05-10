using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static StructureUIValues;
public class ButtonComponent : MonoBehaviour, IStructureUIComponent
{
    [SerializeField] private Button refillButton, upgradeButton;
    [SerializeField] private TextMeshProUGUI refillText, upgradeText;
    [SerializeField] private StructureUIValues refillKey = Refillprice_button;
    [SerializeField] private StructureUIValues upgradeKey = Upgradeprice_button;

    private Func<float> getRefillPrice;
    private Func<float> getUpgradePrice;
    private int price;
    private int ID;

    // Csakis a BuildingUI-hoz fog működni
    public void TrySetup(Dictionary<StructureUIValues, object> data)
    {
        ID = (int)data[StructureUIValues.ID];

        // Refill
        if (data.TryGetValue(refillKey, out var refillObj))
        {
            // beállítjuk a getRefillPrice funkciót
            if (refillObj is Func<float> refillFunc)
                getRefillPrice = refillFunc;
            else
            {
                float fixedPrice = Convert.ToSingle(refillObj);
                getRefillPrice = () => fixedPrice;
            }

            refillButton.gameObject.SetActive(true);
            refillText.gameObject.SetActive(true);
            refillButton.onClick.RemoveAllListeners();
            refillButton.onClick.AddListener(() =>
                FeederManager.Instance.Refill(ID, (int)getRefillPrice()));
        }
        else
        {
            refillButton.gameObject.SetActive(false);
            refillText.gameObject.SetActive(false);
            getRefillPrice = null;
        }

        // Upgrade
        if (data.TryGetValue(upgradeKey, out var upgradeObj))
        {
            if (upgradeObj is Func<float> upgradeFunc)
                getUpgradePrice = upgradeFunc;
            else
            {
                float fixedPrice = Convert.ToSingle(upgradeObj);
                getUpgradePrice = () => fixedPrice;
            }

            upgradeButton.gameObject.SetActive(true);
            upgradeText.gameObject.SetActive(true);
            upgradeButton.onClick.RemoveAllListeners();
            upgradeButton.onClick.AddListener(() => { FacilityManager.Instance.HandleUpgrade(ID, (int)getUpgradePrice()); });


        }
        else
        {
            upgradeButton.gameObject.SetActive(false);
            upgradeText.gameObject.SetActive(false);
            getUpgradePrice = null;
        }
    }

    private void Update()
    {
        // Refill gomb frissítése
        if (getRefillPrice != null)
        {
            float price = getRefillPrice();
            bool canAfford = EconomyManager.Instance.HasEnoughMoney((int)price);
            refillButton.enabled = canAfford && price != 0;
            refillText.text = $"Refill ${price:0}";
        }

        // Upgrade gomb frissítése
        if (getUpgradePrice != null)
        {
            float price = getUpgradePrice();
            bool canAfford = EconomyManager.Instance.HasEnoughMoney((int)price);
            bool canUpgrade = !FacilityManager.Instance.AtMaxLevel(ID);
            upgradeButton.enabled = canAfford && price != 0 && canUpgrade;
            upgradeText.text = canUpgrade ? $"Upgrade ${price:0}" : "Max level";
        }
    }
}

