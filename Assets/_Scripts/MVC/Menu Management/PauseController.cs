using UnityEngine;

public class PauseController : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private void OnEnable()
    {
        InputEventChannel.OnPauseToggled += HandlePauseToggle;
    }

    private void OnDisable()
    {
        InputEventChannel.OnPauseToggled -= HandlePauseToggle;
    }

    private void HandlePauseToggle(bool isPaused)
    {
        pauseMenuUI.SetActive(isPaused);
        Time.timeScale = isPaused ? 0 : 1;
    }

    public void ResumeGame()
    {
        InputEventChannel.RaisePauseToggled(false);
    }

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

    public void ExitToDesktop()
    {
        // Implement alert here when there is unsaved progress...

        Application.Quit();
    }

    public void SaveGame()
    {
        Debug.Log("SaveGame clicked (not implemented).");
    }
}
