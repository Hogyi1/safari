using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// View component for a toggle button. Updates the button's
/// image and text colors based on whether its associated menu
/// is currently active in the UIStackService.
/// </summary>
[RequireComponent(typeof(Image))]
public class ToggleButtonView : MonoBehaviour
{
    [Header("View Colors")]
    /// <summary>Background color of the button when inactive.</summary>
    [SerializeField] private Color normalButtonColor = Color.white;
    /// <summary>Background color of the button when active.</summary>
    [SerializeField] private Color activeButtonColor = Color.green;
    /// <summary>Text color when the button is inactive.</summary>
    [SerializeField] private Color normalTextColor = Color.black;
    /// <summary>Text color when the button is active.</summary>
    [SerializeField] private Color activeTextColor = Color.white;

    // Cached references
    private Image image;
    private TMP_Text text;
    private ToggleButtonController controller;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// Caches component references for image, text, and controller.
    /// </summary>
    private void Awake()
    {
        image = GetComponent<Image>();
        text = GetComponentInChildren<TMP_Text>();
        controller = GetComponent<ToggleButtonController>();
    }

    /// <summary>
    /// OnEnable is called when the object becomes enabled and active.
    /// Triggers an initial visual refresh.
    /// </summary>
    private void OnEnable()
    {
        Refresh();
    }

    /// <summary>
    /// LateUpdate is called every frame, after all Update functions have been called.
    /// Polls the UIStackService and refreshes visuals to keep button state in sync
    /// even when the menu is closed externally (e.g., via ESC or a close button).
    /// </summary>
    private void LateUpdate()
    {
        Refresh();
    }

    /// <summary>
    /// Updates the button's image and text colors based on whether
    /// the controller's target menu is the current top of the stack.
    /// </summary>
    public void Refresh()
    {
        if (controller == null || controller.targetMenu == null)
            return;

        bool isActive = UIStackService.Peek() == controller.targetMenu;
        image.color = isActive ? activeButtonColor : normalButtonColor;
        text.color = isActive ? activeTextColor : normalTextColor;
    }
}
