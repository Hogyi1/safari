using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

/// <summary>
/// Manages the UI elements in the bottom action bar, updating display values and handling time control buttons.
/// </summary>
public class ActionBarManager : MonoBehaviour
{
    [Header("TextFieldsForBottomBar")]
    /// <summary>
    /// Text field displaying the player's current money.
    /// </summary>
    public TMP_Text money;

    /// <summary>
    /// Icon indicating whether money has increased or decreased.
    /// </summary>
    public Image monyUpOrDownIcon;

    /// <summary>
    /// Text field showing the current visitor count.
    /// </summary>
    public TMP_Text visitorCount;

    /// <summary>
    /// Icon indicating whether visitor count has increased or decreased.
    /// </summary>
    public Image visitorsUpOrDownIcon;

    /// <summary>
    /// Text field displaying the park's name.
    /// </summary>
    public TMP_Text parkName;

    /// <summary>
    /// Text field showing the current number of animals.
    /// </summary>
    public TMP_Text animalCount;

    /// <summary>
    /// Icon indicating whether animal count has increased or decreased.
    /// </summary>
    public Image animalUpOrDownIcon;

    [Header("ButtonsForBottomBar")]
    /// <summary>
    /// Button to open the inventory panel.
    /// </summary>
    public Button inventory;

    /// <summary>
    /// Button to open the shop panel.
    /// </summary>
    public Button shop;

    /// <summary>
    /// Button to open the services panel.
    /// </summary>
    public Button services;

    /// <summary>
    /// Button to pause the in-game time.
    /// </summary>
    public Button stopTime;

    /// <summary>
    /// Button to resume the in-game time.
    /// </summary>
    public Button startTime;

    /// <summary>
    /// Button to increase the time speed.
    /// </summary>
    public Button forwardTimeButton;

    /// <summary>
    /// Text field showing the current in-game date and time.
    /// </summary>
    public TMP_Text Date;

    [Header("SpritesForBottomBar")]
    /// <summary>
    /// Sprite used for upward trend icons.
    /// </summary>
    public Sprite upImage;

    /// <summary>
    /// Sprite used for downward trend icons.
    /// </summary>
    public Sprite downImage;

    /// <summary>
    /// Sprite for the first speed-up indicator.
    /// </summary>
    public Sprite forwardImage1;

    /// <summary>
    /// Sprite for the second speed-up indicator.
    /// </summary>
    public Sprite forwardImage2;

    /// <summary>
    /// Sprite for the third speed-up indicator.
    /// </summary>
    public Sprite forwardImage3;

    /// <summary>
    /// Updates UI text fields each frame with the latest economy and time data.
    /// </summary>
    private void Update()
    {
        money.text = Convert.ToString(EconomyManager.Instance.GetEconomy().CurrentMoney);
        animalCount.text = "100"; // TODO: Replace hardcoded value with dynamic data
        visitorCount.text = "100"; // TODO: Replace hardcoded value with dynamic data
        Date.text = TimeManager.Instance.GetCurrentTime().ToString();
    }

    /// <summary>
    /// Initializes button listeners for pausing, resuming, and speeding up time.
    /// </summary>
    public void Start()
    {
        parkName.text = Park.Instance.ParkName;
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
