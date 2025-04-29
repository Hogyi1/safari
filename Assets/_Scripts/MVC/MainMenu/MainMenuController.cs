using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{

    [Header("Menu Navigation")]
    [SerializeField] private SaveSlotsMenu saveSlotsMenu;
    [SerializeField] GameObject[] menus;
    [SerializeField] private Button continueGameButton;
    [SerializeField] private Button loadGameButton;
    [SerializeField] private string gameSceneName = "Bemutato";

    private void Start()
    {
        mm_HideAllMenus();
        DisableButtonsDependingOnData();
    }

    public void mm_NavigationBarClick(GameObject activeMenu)
    {
        foreach (GameObject menu in menus)
        {
            menu.SetActive(false);
        }
        activeMenu.SetActive(true);  
    }

    public void mm_Continue()
    {
        if (SceneHandler.Instance != null)
        {

            DataPersistenceManager.Instance.SaveGame();
            SceneHandler.Instance.LoadGameScene(gameSceneName);
        }
        else
        {
            Debug.LogWarning("SceneHandler.Instance is null. Can't load menu.");
        }
    }

    public void mm_ExitToDesktop()
    {
        Application.Quit();

        Debug.Log("Application closed.");
    }

    public void mm_HideAllMenus()
    {
        foreach (GameObject menu in menus)
        {
            menu.SetActive(false);
        }
    }

    public void OnContinueGameClicked()
    {
        // save the game anytime before loading a new scene
        DataPersistenceManager.Instance.SaveGame();
        // load the next scene - which will in turn load the game because of 
        // OnSceneLoaded() in the DataPersistenceManager
        SceneManager.LoadSceneAsync("SampleScene");
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
