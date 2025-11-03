using UnityEngine;

/// <summary>
/// Controls settings menu panels, allowing switching and default panel initialization.
/// </summary>
public class SettingsMenuController : MonoBehaviour
{
    /// <summary>
    /// Array of settings panels to toggle.
    /// </summary>
    [SerializeField] GameObject[] panels;

    /// <summary>
    /// The panel shown by default on start.
    /// </summary>
    private GameObject defaultPanel;

    /// <summary>
    /// Initializes settings panels by hiding all and showing the default.
    /// </summary>
    private void Start()
    {
        sm_HideAllPanels();

        // Set default panel
        defaultPanel = panels[0];

        // Show default panel
        defaultPanel.SetActive(true);
    }

    /// <summary>
    /// Activates the selected settings panel and deactivates all others.
    /// </summary>
    /// <param name="activePanel">The panel GameObject to show.</param>
    public void sm_NavigationBarClick(GameObject activePanel)
    {
        foreach (GameObject panel in panels)
        {
            panel.SetActive(false);
        }
        activePanel.SetActive(true);
    }

    /// <summary>
    /// Hides all settings panels.
    /// </summary>
    private void sm_HideAllPanels()
    {
        foreach (GameObject panel in panels)
        {
            panel.SetActive(false);
        }
    }
}
