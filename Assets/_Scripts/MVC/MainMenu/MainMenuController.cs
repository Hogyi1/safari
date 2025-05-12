using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Controls the main menu panels and game navigation actions such as continue and exit.
/// </summary>
public class MainMenuController : MonoBehaviour
{
    [Header("Menu Navigation")]

    /// <summary>
    /// Reference to the save slots menu used for selecting save files.
    /// </summary>
    [SerializeField] private SaveSlotsMenu saveSlotsMenu;

    /// <summary>
    /// Array of menu GameObjects to toggle during navigation.
    /// </summary>
    [SerializeField] private GameObject[] menus;

    /// <summary>
    /// Button used to continue the most recently saved game.
    /// </summary>
    [SerializeField] private Button continueGameButton;

    /// <summary>
    /// Button used to load an existing save game from the save slots menu.
    /// </summary>
    [SerializeField] private Button loadGameButton;

    /// <summary>
    /// The name of the main game scene to load when starting or continuing a game.
    /// </summary>
    [SerializeField] private string gameSceneName = "Bemutato";

    /// <summary>
    /// Hides all menu panels on start.
    /// </summary>
    private void Start()
    {
        mm_HideAllMenus();
        DisableButtonsDependingOnData();
    }

    /// <summary>
    /// Activates the selected menu and deactivates all others.
    /// </summary>
    /// <param name="activeMenu">The menu GameObject to show.</param>
    public void mm_NavigationBarClick(GameObject activeMenu)
    {
        foreach (GameObject menu in menus)
        {
            menu.SetActive(false);
        }
        activeMenu.SetActive(true);  
    }

    /// <summary>
    /// Continues the game by loading the specified game scene.
    /// </summary>
    public void mm_Continue()
    {
        if (SceneHandler.Instance != null)
        {
            DataPersistenceManager.Instance.LoadGame();
            SceneHandler.Instance.LoadGameScene(gameSceneName);
        }
        else
        {
            Debug.LogWarning("SceneHandler.Instance is null. Can't load menu.");
        }
    }

    /// <summary>
    /// Exits the application and logs the closure.
    /// </summary>
    public void mm_ExitToDesktop()
    {
        Application.Quit();

        Debug.Log("Application closed.");
    }

    /// <summary>
    /// Hides all registered menu panels.
    /// </summary>
    public void mm_HideAllMenus()
    {
        foreach (GameObject menu in menus)
        {
            menu.SetActive(false);
        }
    }

    /// <summary>
    /// Called when the "New Game" button is clicked.
    /// Activates the save slots menu for starting a new game.
    /// </summary>
    public void OnNewGameClicked()
    {
        saveSlotsMenu.ActivateMenu(false);
    }

    /// <summary>
    /// Called when the "Load Game" button is clicked.
    /// Activates the save slots menu for loading an existing game.
    /// </summary>
    public void OnLoadGameClicked()
    {
        saveSlotsMenu.ActivateMenu(true);
    }

    /// <summary>
    /// Disables the "Continue" and "Load Game" buttons
    /// if no game data is found in the current profile.
    /// </summary>
    private void DisableButtonsDependingOnData()
    {
        if (!DataPersistenceManager.Instance.HasGameData())
        {
            continueGameButton.interactable = false;
            loadGameButton.interactable = false;
        }
    }

}
