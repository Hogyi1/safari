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
        if (string.IsNullOrEmpty(sceneToLoad) || sceneToLoad == "MainMenu")
        {
            sceneToLoad = "MainMenu";
            StartCoroutine(PerformLoading(sceneToLoad));
        }
        else
            StartCoroutine(LoadSceneAdditively(sceneToLoad));
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
        try
        {
            Shader.WarmupAllShaders();
        }
        catch { }
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

    private IEnumerator LoadSceneAdditively(string sceneToLoad)
    {
        StartCoroutine(view.Spin());

        try
        {
            Shader.WarmupAllShaders();
        }
        catch { }

        // Load the target scene in additive mode (will not replace the loading scene)
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);

        while (!op.isDone)
        {
            yield return null;
        }

        // Wait for DataPersistenceManager to finish loading everything
        bool dataLoaded = false;
        DataPersistenceManager.Instance.OnAllLoaded += () => dataLoaded = true;

        yield return new WaitUntil(() => dataLoaded);

        // Optional wait
        yield return new WaitForSeconds(0.5f);

        // Set the new scene as active
        Scene loadedScene = SceneManager.GetSceneByName(sceneToLoad);
        if (loadedScene.IsValid())
        {
            SceneManager.SetActiveScene(loadedScene);
        }

        // Unload the loading scene
        SceneManager.UnloadSceneAsync("Loading");
    }
}
