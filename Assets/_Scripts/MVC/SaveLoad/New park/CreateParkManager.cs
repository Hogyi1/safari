using UnityEngine;

/// <summary>
/// Manages the logic for handling park creation via the UI,
/// including responding to user input and navigating to the save slots menu.
/// </summary>
public class CreateParkManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the CreateParkManager.
    /// </summary>
    public static CreateParkManager Instance;

    [Header("References")]
    /// <summary>
    /// Reference to the view component handling park creation input.
    /// </summary>
    public CreateParkView view;

    /// <summary>
    /// Reference to the save slots menu UI.
    /// </summary>
    public SaveSlotsMenu saveSlotsMenu;

    /// <summary>
    /// UI container used for displaying load and new game options.
    /// </summary>
    public GameObject LoadAndNew;

    /// <summary>
    /// Reference to the Main menu controller
    /// </summary>
    public MainMenuController MainMenu;

    /// <summary>
    /// Ensures a single instance of the CreateParkManager exists.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Handles the event when the user confirms park creation,
    /// setting park properties and switching to the save slots menu.
    /// </summary>
    /// <param name="parkName">The validated name of the park.</param>
    /// <param name="difficulty">The selected difficulty level.</param>
    public void HandleParkConfirmed(string parkName, DifficultyEnum difficulty)
    {
        Park.Instance.ParkName = parkName;
        Park.Instance.Difficulty = difficulty;

        MainMenu.mm_NavigationBarClick(LoadAndNew);
        saveSlotsMenu.ActivateMenu(false); // false = new game mode
    }
}
