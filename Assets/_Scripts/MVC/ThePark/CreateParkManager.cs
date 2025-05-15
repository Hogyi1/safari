using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the park creation UI and logic, including input validation and difficulty selection.
/// </summary>
public class CreateParkManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the CreateParkManager.
    /// </summary>
    public static CreateParkManager Instance;

    /// <summary>
    /// Ensures only one instance exists at runtime.
    /// </summary>
    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    /// <summary>
    /// Reference to the main menu controller.
    /// </summary>
    public MainMenuController MainMenu;

    /// <summary>
    /// Reference to the save slots menu controller.
    /// </summary>
    public SaveSlotsMenu SaveSlotsMenu;

    /// <summary>
    /// UI container used to switch between load and new park creation screens.
    /// </summary>
    public GameObject LoadAndNew;

    /// <summary>
    /// Input field for entering the park name.
    /// </summary>
    public TMP_InputField inputField;

    /// <summary>
    /// Dropdown for selecting the difficulty level.
    /// </summary>
    public TMP_Dropdown dropdown;

    /// <summary>
    /// Button that triggers validation and park creation.
    /// </summary>
    public Button button;

    /// <summary>
    /// Stores the user-entered park name after validation.
    /// </summary>
    private string ParkName;

    /// <summary>
    /// Stores the selected difficulty level.
    /// </summary>
    private DifficultyEnum difficulty;

    /// <summary>
    /// Validates the user input and proceeds to set the park properties if valid.
    /// Shows a popup if the park name is empty.
    /// </summary>
    public void ValidateInput()
    {
        string inputText = inputField.text.Trim();
        difficulty = (DifficultyEnum)dropdown.value;

        if (string.IsNullOrEmpty(inputText))
        {
            GameEvents.Instance.RequestAlert(
                success: false,
                message: "The park name can't be empty",
                fadeInTime: 0.25f,
                displayTime: 2.5f,
                fadeOutTime: 0.4f
            );  
            return;
        }

        ParkName = inputText;
        SetParkPropertys();
        MainMenu.mm_NavigationBarClick(LoadAndNew);
        SaveSlotsMenu.ActivateMenu(false);
    }

    /// <summary>
    /// Applies the validated park name and difficulty to the Park singleton instance.
    /// </summary>
    public void SetParkPropertys()
    {
        Park.Instance.ParkName = ParkName;
        Park.Instance.difficulty = difficulty;
    }
}
