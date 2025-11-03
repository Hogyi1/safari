using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Singleton for animating UI elements.
/// Call Animate.Instance.ProgressBarAnim(...) to smoothly tween
/// an Image.fillAmount from its current value to a target over time.
/// </summary>
public class Animate : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the Animate helper for global access.
    /// </summary>
    public static Animate Instance { get; private set; }

    /// <summary>
    /// Ensures only one instance of Animate exists and persists across scenes.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Keep track of running coroutines per Image so we can cancel them
    private readonly Dictionary<Image, Coroutine> _running = new Dictionary<Image, Coroutine>();
    private readonly Dictionary<CanvasGroup, Coroutine> _runningFade = new Dictionary<CanvasGroup, Coroutine>();

    /// <summary>
    /// Smoothly animates the given Image.fillAmount from its current value
    /// to targetFill (0–1) over duration seconds.
    /// </summary>
    /// <param name="progressBar">
    /// The UI Image (Fill Method = Filled) to animate.
    /// </param>
    /// <param name="targetFill">
    /// Final fill amount, between 0 and 1.
    /// </param>
    /// <param name="duration">
    /// How many seconds the tween should take.
    /// </param>
    public void ProgressBarAnim(Image progressBar, float targetFill, float duration)
    {
        if (progressBar == null)
            return;

        // Stop any in-flight animation on this bar
        if (_running.TryGetValue(progressBar, out var runningCoroutine))
            StopCoroutine(runningCoroutine);

        // Start a new one
        var c = StartCoroutine(ProgressBarRoutine(progressBar, targetFill, duration));
        _running[progressBar] = c;
    }

    /// <summary>
    /// Coroutine that linearly interpolates fillAmount.
    /// </summary>
    private IEnumerator ProgressBarRoutine(Image bar, float target, float duration)
    {
        float start = bar.fillAmount;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            bar.fillAmount = Mathf.Lerp(start, target, t);
            yield return null;
        }

        bar.fillAmount = target;
        _running.Remove(bar);
    }

    // FadeIn and FadeOut needs more work, it is not functional.

    /// <summary>
    /// Fades in the specified CanvasGroup over duration seconds.
    /// Enables the GameObject before fading, then sets interactability.
    /// </summary>
    public void FadeIn(CanvasGroup cg, float duration, Action onComplete = null)
    {
        if (cg == null) return;
        cg.gameObject.SetActive(true);
        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;

        if (_runningFade.TryGetValue(cg, out var existing)) StopCoroutine(existing);
        _runningFade[cg] = StartCoroutine(FadeRoutine(cg, 0f, 1f, duration, () =>
        {
            cg.interactable = true;
            cg.blocksRaycasts = true;
            onComplete?.Invoke();
        }));
    }

    /// <summary>
    /// Fades out the specified CanvasGroup over duration seconds.
    /// Disables interactability immediately, then deactivates GameObject on complete.
    /// </summary>
    public void FadeOut(CanvasGroup cg, float duration, Action onComplete = null)
    {
        if (cg == null) return;
        cg.interactable = false;
        cg.blocksRaycasts = false;

        if (_runningFade.TryGetValue(cg, out var existing)) StopCoroutine(existing);
        _runningFade[cg] = StartCoroutine(FadeRoutine(cg, cg.alpha, 0f, duration, () =>
        {
            cg.gameObject.SetActive(false);
            onComplete?.Invoke();
        }));
    }

    private IEnumerator FadeRoutine(CanvasGroup cg, float from, float to, float duration, Action onComplete)
    {
        float elapsed = 0f;
        cg.alpha = from;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(from, to, Mathf.Clamp01(elapsed / duration));
            yield return null;
        }
        cg.alpha = to;
        _runningFade.Remove(cg);
        onComplete?.Invoke();
    }
}
