using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controls the loading process: warming shaders, loading the target scene,
/// and activating it once ready.
/// </summary>
public class LoadingController : MonoBehaviour
{
    /// <summary>
    /// Reference to the LoadingView that manages UI fades and spinner.
    /// </summary>
    [SerializeField]
    private LoadingView view;

    /// <summary>
    /// Called on MonoBehaviour startup. Retrieves the next scene name
    /// (or defaults to MainMenu if none) and begins the loading coroutine.
    /// </summary>
    private void Start()
    {
        string sceneToLoad = SceneLoadManager.NextSceneName;
        if (string.IsNullOrEmpty(sceneToLoad))
        {
            sceneToLoad = "MainMenu";
        }
        StartCoroutine(PerformLoading(sceneToLoad));
    }

    /// <summary>
    /// Coroutine that handles warming shaders, loading the scene asynchronously,
    /// and activating it when loading is complete.
    /// </summary>
    /// <param name="sceneToLoad">Name of the scene to load.</param>
    /// <returns>IEnumerator for the coroutine.</returns>
    private IEnumerator PerformLoading(string sceneToLoad)
    {
        // Start the spinner animation
        StartCoroutine(view.Spin());

        // Warm up all shaders to avoid runtime hitches
        Shader.WarmupAllShaders();

        // Begin loading the target scene asynchronously (additive if desired)
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneToLoad);
        op.allowSceneActivation = false;  // Stall at 90% until we're ready

        // Wait until load progress reaches at least 90%
        while (op.progress < 0.9f)
            yield return null;

        // Optional small buffer before activating the scene
        yield return new WaitForSeconds(2f);

        // Activate the loaded scene
        op.allowSceneActivation = true;
    }
}
