using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the visual loading UI: spinner rotation and optional fades.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class LoadingView : MonoBehaviour
{
    /// <summary>
    /// CanvasGroup used for fading the UI in and out.
    /// </summary>
    [SerializeField]
    private CanvasGroup canvasGroup;

    /// <summary>
    /// Image component of the spinner to be rotated.
    /// </summary>
    [SerializeField]
    private Image spinner;

    /// <summary>
    /// Duration, in seconds, of any fade animations.
    /// </summary>
    [SerializeField]
    private float fadeDuration = 0.5f;

    /// <summary>
    /// Initializes the CanvasGroup alpha to fully opaque on awake.
    /// </summary>
    private void Awake()
    {
        canvasGroup.alpha = 1f;
    }

    /// <summary>
    /// Coroutine that continuously rotates the spinner image.
    /// </summary>
    /// <returns>IEnumerator for the spinner coroutine.</returns>
    public IEnumerator Spin()
    {
        while (true)
        {
            spinner.transform.Rotate(0, 0, -360f * Time.deltaTime);
            yield return null;
        }
    }
}
