using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Singleton for managing scene loading operations.
/// </summary>
public class SceneHandler : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the SceneHandler.
    /// </summary>
    public static SceneHandler Instance;

    /// <summary>
    /// Ensures a single instance and persists across scenes.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Loads the specified scene by name.
    /// </summary>
    /// <param name="name">Name of the scene to load.</param>
    public void LoadGameScene(string name)
    {
        SceneLoadManager.LoadScene(name);
    }
}
