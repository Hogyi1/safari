using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controller component for a toggle button. Handles user interactions,
/// determines which menu zone to target, and instructs the MenuManager
/// to show or hide the associated UI panel.
/// </summary>
[RequireComponent(typeof(Button))]
public class ToggleButtonController : MonoBehaviour
{
    /// <summary>
    /// Defines the group or area of the UI that this button controls.
    /// </summary>
    public enum Zone
    {
        SideMenu,
        ActionBar,
        Other
    }

    [Header("Controller Settings")]
    /// <summary>
    /// The zone this button belongs to, determining which MenuManager
    /// method will be called when toggling.
    /// </summary>
    [SerializeField]
    private Zone zone = Zone.Other;

    /// <summary>
    /// The UI panel GameObject that this button will toggle on and off.
    /// </summary>
    public GameObject targetMenu;

    [Header("Animation Settings")]
    [SerializeField]
    private float fadeDuration = 0.2f;

    // Cached references
    private Button button;
    private MenuManager menuManager;
    private ToggleButtonView view;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// Caches required component references and wires up the click listener.
    /// </summary>
    private void Awake()
    {
        button = GetComponent<Button>();
        menuManager = FindFirstObjectByType<MenuManager>();
        view = GetComponent<ToggleButtonView>();
        button.onClick.AddListener(OnClick);
    }

    /// <summary>
    /// Handles button click events, toggles the target menu
    /// using the appropriate MenuManager call based on the zone,
    /// and requests the view to refresh its visual state.
    /// </summary>
    private void OnClick()
    {
        if (menuManager == null || targetMenu == null)
            return;

        bool isOpen = UIStackService.Peek() == targetMenu;

        switch (zone)
        {
            case Zone.SideMenu:
                if (isOpen)
                    menuManager.HideMenu(targetMenu);
                else
                    menuManager.SideMenuNavigationClick(targetMenu);
                break;

            case Zone.ActionBar:
                if (isOpen)
                    menuManager.HideMenu(targetMenu);
                else
                    menuManager.ActionBarNavigationClick(targetMenu);
                break;

            default:
                if (isOpen)
                    menuManager.HideMenu(targetMenu);
                else
                    menuManager.ShowMenu(targetMenu);
                break;
        }

        if (view != null)
            view.Refresh();
    }
}