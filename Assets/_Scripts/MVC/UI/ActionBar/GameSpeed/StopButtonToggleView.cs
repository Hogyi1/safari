using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
/// <summary>
/// A view component for the Stop button that toggles its appearance based on whether the game is paused.
/// </summary>
public class StopButtonToggleView : MonoBehaviour
{
    [Header("View Colors")]
    /// <summary>
    /// Background color of the button when the game is not paused (i.e., inactive).
    /// </summary>
    [SerializeField] private Color normalButtonColor = Color.white;
    /// <summary>
    /// Background color of the button when the game is paused (i.e., active).
    /// </summary>
    [SerializeField] private Color activeButtonColor = Color.green;

    [Header("Images")]
    /// <summary>
    /// Reference to the background Image component of the button.
    /// </summary>
    [SerializeField] private Image image;
    /// <summary>
    /// Reference to the icon Image component displayed on the button.
    /// </summary>
    [SerializeField] private Image icon;

    /// <summary>
    /// Called by Unity when the component is first initialized.
    /// Ensures the button's appearance is correct on startup.
    /// </summary>
    private void Start()
    {
        Refresh();
    }

    /// <summary>
    /// Called by Unity once per frame, after all Update methods.
    /// Ensures the button's appearance stays in sync with the pause state.
    /// </summary>
    private void LateUpdate()
    {
        Refresh();
    }

    /// <summary>
    /// Updates the button's background and icon colors based on the current pause state.
    /// </summary>
    public void Refresh()
    {
        bool isPaused = TimeManager.Instance.IsPaused;
        image.color = isPaused ? activeButtonColor : normalButtonColor;
        icon.color = isPaused ? Color.white : Color.black;
    }
}
