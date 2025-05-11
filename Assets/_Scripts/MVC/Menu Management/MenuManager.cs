using UnityEngine;

/// <summary>
/// Manages show/hide operations for UI panels in the side menu and action bar.
/// Integrates with UIStackService to track currently open UI panels.
/// </summary>
public class MenuManager : MonoBehaviour
{
    /// <summary>
    /// List of side menu panels that can be toggled.
    /// </summary>
    [SerializeField] GameObject[] panels;

    /// <summary>
    /// List of action bar panels that can be toggled.
    /// </summary>
    [SerializeField] GameObject[] actionBarPanels;

    /// <summary>
    /// Activates the specified menu GameObject.
    /// </summary>
    /// <param name="menu">The menu GameObject to show.</param>
    public void ShowMenu(GameObject menu)
    {
        UIStackService.Push(menu);
    }

    /// <summary>
    /// Deactivates the specified menu GameObject.
    /// </summary>
    /// <param name="menu">The menu GameObject to hide.</param>
    public void HideMenu(GameObject menu)
    {
        if (UIStackService.Peek() == menu)
        {
            UIStackService.Pop();
        }
        else
        {
            menu.SetActive(false);
        }
    }

    /// <summary>
    /// Disables all side menu panels then enables the specified panel.
    /// </summary>
    /// <param name="activePanel">The side menu panel to activate.</param>
    public void SideMenuNavigationClick(GameObject activePanel)
    {
        foreach (GameObject panel in panels)
        {
            if (UIStackService.Peek() == panel)
                UIStackService.Pop();
            else
                panel.SetActive(false);
        }
        UIStackService.Push(activePanel);
    }

    /// <summary>
    /// Disables all action bar panels then enables the specified panel.
    /// </summary>
    /// <param name="activePanel">The action bar panel to activate.</param>
    public void ActionBarNavigationClick(GameObject activePanel)
    {
        foreach (GameObject panel in actionBarPanels)
        {
            if (UIStackService.Peek() == panel)
                UIStackService.Pop();
            else
                panel.SetActive(false);
        }
        UIStackService.Push(activePanel);
    }
}
