using UnityEngine;

/// <summary>
/// (Optional) Data container for asynchronous loading operations,
/// can be expanded to track progress or expose additional states.
/// </summary>
public class LoadingModel
{
    /// <summary>
    /// The AsyncOperation returned by SceneManager.LoadSceneAsync.
    /// </summary>
    public AsyncOperation Operation { get; set; }
}
