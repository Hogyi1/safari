using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] GameObject[] menus;

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
        // Continue button behaviour...

        Debug.Log("Continue button clicked.");
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
