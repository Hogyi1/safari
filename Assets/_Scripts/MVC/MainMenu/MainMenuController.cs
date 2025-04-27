using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] GameObject[] menus;
    [SerializeField] private string gameSceneName = "Bemutato";

    private void Start()
    {
        mm_HideAllMenus();
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
}
