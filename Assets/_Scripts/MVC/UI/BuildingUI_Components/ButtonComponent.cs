using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static StructureUIValues;

/// <summary>
/// Component providing refill and upgrade button functionality
/// within a structure popup UI, displaying dynamic prices
/// and handling button interactivity.
/// </summary>
public class ButtonComponent : MonoBehaviour, IStructureUIComponent
{
    /// <summary>
    /// Button used to trigger action refill or upgrade action.
    /// </summary>
    [SerializeField] private Button refillButton, upgradeButton;

    /// <summary>
    /// Text label for the refill and upgrade button showing its price.
    /// </summary>
    [SerializeField] private TextMeshProUGUI refillText, upgradeText;

    /// <summary>
    /// Key used to retrieve refill price from popup data.
    /// </summary>
    [SerializeField] private StructureUIValues refillKey = Refillprice_button;

    /// <summary>
    /// Key used to retrieve upgrade price from popup data.
    /// </summary>
    [SerializeField] private StructureUIValues upgradeKey = Upgradeprice_button;

    private Func<float> getRefillPrice;
    private Func<float> getUpgradePrice;
<<<<<<< HEAD
    private int price;
    private int ID;
=======
>>>>>>> 923712d8f96365157209ba049298a2f508440c16

    /// <summary>
    /// Configures button visibility, price retrieval functions,
    /// and click listeners based on provided popup data.
    /// </summary>
    /// <param name="data">Dictionary mapping UI value keys to dynamic data.</param>
    public void TrySetup(Dictionary<StructureUIValues, object> data)
    {
        ID = (int)data[StructureUIValues.ID];

        // Refill
        if (data.TryGetValue(refillKey, out var refillObj))
        {
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

        // Upgrade button setup
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

    /// <summary>
    /// Updates button interactivity and price text each frame
    /// based on current economy and dynamic price functions.
    /// </summary>
    private void Update()
    {
        // Refill button
        if (getRefillPrice != null)
        {
            float price = getRefillPrice();
            bool canAfford = EconomyManager.Instance.HasEnoughMoney((int)price);
            refillButton.enabled = canAfford && price != 0;
            refillText.text = $"Refill ${price:0}";
        }

        // Upgrade button
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

