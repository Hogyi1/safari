using UnityEngine;
using UnityEngine.UIElements;

public class SettingsMenuController : MonoBehaviour
{
    [SerializeField] GameObject[] panels;
    private GameObject defaultPanel;

    private void Start()
    {
        sm_HideAllPanels();

        // Set default panel
        defaultPanel = panels[0];

        // Show default panel
        defaultPanel.SetActive(true);
    }

    public void sm_NavigationBarClick(GameObject activePanel)
    {
        foreach (GameObject panel in panels)
        {
            panel.SetActive(false);
        }
        activePanel.SetActive(true);
    }

    private void sm_HideAllPanels()
    {
        foreach (GameObject panel in panels)
        {
            panel.SetActive(false);
        }
    }
}
