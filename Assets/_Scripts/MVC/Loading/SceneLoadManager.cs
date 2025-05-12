using UnityEngine.SceneManagement;

public static class SceneLoadManager
{
    // Name of the scene to load after the loading screen
    public static string NextSceneName { get; private set; }

    // Call this from any UI button or controller to initiate a scene load
    public static void LoadScene(string sceneName)
    {
        NextSceneName = sceneName;
        SceneManager.LoadScene("Loading");  // Loading scene must be at build index 0
    }
}