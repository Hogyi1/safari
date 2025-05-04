using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class AlertView : MonoBehaviour
{
    public event Action<AlertView> OnFadeOutComplete;

    [Header("UI References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text messageText;
    private CanvasGroup canvasGroup;

    /// <summary>
    /// Caches the CanvasGroup component used for fade animations.
    /// </summary>
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    /// <summary>
    /// Configure and play the alert animation.
    /// </summary>
    /// <param name="success">True = check icon, False = X icon</param>
    /// <param name="message">The message to display</param>
    /// <param name="fadeInTime">Seconds to fade in</param>
    /// <param name="displayTime">Seconds to stay fully visible</param>
    /// <param name="fadeOutTime">Seconds to fade out</param>
    public void Setup(
        bool success,
        string message,
        float fadeInTime,
        float displayTime,
        float fadeOutTime)
    {
        // Assign icon
        iconImage.sprite = success ? AlertIcons.Instance.checkSprite : AlertIcons.Instance.crossSprite;

        // Assign text
        messageText.text = message;

        // Start transparent
        canvasGroup.alpha = 0f;

        // Start coroutine
        StartCoroutine(PlaySequence(fadeInTime, displayTime, fadeOutTime));
    }

    /// <summary>
    /// Coroutine that plays the full alert animation sequence: fade-in, hold, and fade-out.
    /// </summary>
    /// <param name="fadeIn">Duration in seconds to fade from transparent to fully visible.</param>
    /// <param name="hold">Duration in seconds to remain fully visible.</param>
    /// <param name="fadeOut">Duration in seconds to fade from fully visible to transparent.</param>
    /// <returns>IEnumerator for coroutine execution.</returns>
    private IEnumerator PlaySequence(float fadeIn, float hold, float fadeOut)
    {
        // Fade in
        yield return Fade(0f, 1f, fadeIn);

        // Hold
        yield return new WaitForSeconds(hold);

        // Fade out
        yield return Fade(1f, 0f, fadeOut);

        // Notify controller
        OnFadeOutComplete?.Invoke(this);
    }

    /// <summary>
    /// Coroutine that interpolates the CanvasGroup's alpha over a given duration.
    /// </summary>
    /// <param name="from">Starting alpha value.</param>
    /// <param name="to">Target alpha value.</param>
    /// <param name="duration">Time in seconds over which to interpolate.</param>
    /// <returns>IEnumerator for coroutine execution.</returns>
    private IEnumerator Fade(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        canvasGroup.alpha = to;
    }
}