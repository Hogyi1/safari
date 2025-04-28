using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] GameObject[] panels;

    public void ShowMenu(GameObject menu)
    {
        menu.SetActive(true);
    }

    public void HideMenu(GameObject menu)
    {
        menu.SetActive(false);
    }

    /// <summary>
    /// Disables all panels in panels list, then enables the selected one.
    /// </summary>
    /// <param name="activePanel"></param>
    public void SideMenuNavigationClick(GameObject activePanel)
    {
        foreach (GameObject panel in panels)
        {
            panel.SetActive(false);
        }
        activePanel.SetActive(true);
    }
}
