using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Controls the main menu panels and game navigation actions such as continue and exit.
/// </summary>
public class MainMenuController : MonoBehaviour
{
    [Header("Menu Navigation")]
    [SerializeField] private SaveSlotsMenu saveSlotsMenu;
    /// <summary>
    /// Array of menu GameObjects to toggle on navigation.
    /// </summary>
    [SerializeField] GameObject[] menus;
    [SerializeField] private Button continueGameButton;
    [SerializeField] private Button loadGameButton;
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

    public void OnNewGameClicked()
    {
        saveSlotsMenu.ActivateMenu(false);
    }

    public void OnLoadGameClicked()
    {
        saveSlotsMenu.ActivateMenu(true);
    }

    private void DisableButtonsDependingOnData()
    {
        if (!DataPersistenceManager.Instance.HasGameData())
        {
            continueGameButton.interactable = false;
            loadGameButton.interactable = false;
        }
    }
}
