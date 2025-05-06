using UnityEngine;

/// <summary>
/// Manages the pause menu UI and handles pausing/resuming the game.
/// </summary>
public class PauseController : MonoBehaviour
{
    /// <summary>
    /// The UI GameObject representing the pause menu panel.
    /// </summary>
    [SerializeField] private GameObject pauseMenuUI;

    /// <summary>
    /// The name of the main menu scene to load when exiting to main menu.
    /// </summary>
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    /// <summary>
    /// Subscribes to pause toggle events when this component is enabled.
    /// </summary>
    private void OnEnable()
    {
        InputEventChannel.OnPauseToggled += HandlePauseToggle;
    }

    /// <summary>
    /// Unsubscribes from pause toggle events when this component is disabled.
    /// </summary>
    private void OnDisable()
    {
        InputEventChannel.OnPauseToggled -= HandlePauseToggle;
    }

    /// <summary>
    /// Handles the pause toggle event by showing or hiding the pause menu
    /// and adjusting the time scale accordingly.
    /// </summary>
    /// <param name="isPaused">True if the game is paused; false otherwise.</param>
    private void HandlePauseToggle(bool isPaused)
    {
        pauseMenuUI.SetActive(isPaused);
        Time.timeScale = isPaused ? 0 : 1;
    }

    /// <summary>
    /// Resumes the game by raising the pause toggled event with false.
    /// </summary>
    public void ResumeGame()
    {
        InputEventChannel.RaisePauseToggled(false);
    }

    /// <summary>
    /// Exits to the main menu, resets time scale, and loads the main menu scene.
    /// </summary>
    public void ExitToMainMenu()
    {
        // Implement alert here when there is unsaved progress...

        Time.timeScale = 1;
        if (SceneHandler.Instance != null)
        {
            SceneHandler.Instance.LoadGameScene(mainMenuSceneName);
        }
        else
        {
            Debug.LogWarning("SceneHandler.Instance is null. Can't load menu.");
        }

    }

    /// <summary>
    /// Exits the application to the desktop.
    /// </summary>
    public void ExitToDesktop()
    {
        // Implement alert here when there is unsaved progress...

        Application.Quit();
    }

    /// <summary>
    /// Saves the game state (not yet implemented).
    /// </summary>
    public void SaveGame()
    {
        Debug.Log("SaveGame clicked (not implemented).");
    }
}
