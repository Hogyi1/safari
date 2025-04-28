using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;


public class ActionBarManager : MonoBehaviour
{
    [Header("TextFieldsForBottomBar")]
    public TMP_Text money;
    public Image monyUpOrDownIcon;
    public TMP_Text visitorCount;
    public Image visitorsUpOrDownIcon;
    public TMP_Text parkName;
    public TMP_Text animalCount;
    public Image animalUpOrDownIcon;

    [Header("ButtonsForBottomBar")]
    public Button inventory;
    public Button shop;
    public Button services;
    public Button stopTime;
    public Button startTime;
    public Button forwardTimeButton;
    public TMP_Text Date;

    [Header("SpritesForBottomBar")]
    public Sprite upImage;
    public Sprite downImage;
    public Sprite forwardImage1;
    public Sprite forwardImage2;
    public Sprite forwardImage3;

    private void Update()
    {
        money.text =  Convert.ToString(EconomyManager.Instance.getEconomy().CurrentMoney);
        animalCount.text = "100";
        visitorCount.text = "100";
        Date.text = TimeManager.Instance.GetCurrentTime().ToString();
    }

    public void Start()
    {
        // ide kell a datapersistancebol a parkname parkName.text = 
        stopTime.onClick.AddListener(() =>
        {
            TimeManager.Instance.PauseTime();
        });

        startTime.onClick.AddListener(() =>
        {
            TimeManager.Instance.ResumeTime();
        });
        forwardTimeButton.onClick.AddListener(() =>
        {
            TimeManager.Instance.SpeedUpTime();
        });
    }

}
