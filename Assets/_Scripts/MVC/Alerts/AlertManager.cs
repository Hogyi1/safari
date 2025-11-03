using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controller for queuing and displaying alert notifications.
/// </summary>
public class AlertManager : MonoBehaviour
{
    public static AlertManager Instance { get; private set; }

    [Header("Alert Prefab & Parent")]
    [SerializeField] private AlertView alertPrefab;
    [SerializeField] private Transform alertsParent;

    [Header("Queue Settings")]
    [SerializeField] private int maxConcurrentAlerts = 1;

    private readonly Queue<AlertRequest> alertQueue = new Queue<AlertRequest>();
    private int currentActiveAlerts = 0;

    /// <summary>
    /// Ensures only one instance of AlertManager exists and persists across scenes.
    /// Subscribe to GameEvents.
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

        GameEvents.Instance.OnAlertRequested += ShowAlert;
    }

    /// <summary>
    /// Public API to request a new alert.
    /// Will display immediately if under the concurrent limit, or enqueue otherwise.
    /// </summary>
    /// <param name="success">True for check icon, false for cross icon.</param>
    /// <param name="message">Alert message text.</param>
    /// <param name="fadeInTime">Seconds to fade in.</param>
    /// <param name="displayTime">Seconds to stay visible.</param>
    /// <param name="fadeOutTime">Seconds to fade out.</param>
    public void ShowAlert(bool success, string message, float fadeInTime, float displayTime, float fadeOutTime)
    {
        var req = new AlertRequest(success, message, fadeInTime, displayTime, fadeOutTime);
        if (currentActiveAlerts < maxConcurrentAlerts)
        {
            DisplayAlert(req);
        }
        else
        {
            alertQueue.Enqueue(req);
        }
    }

    /// <summary>
    /// Instantiates and configures a new alert view from the queued request.
    /// </summary>
    /// <param name="req">
    /// An <see cref="AlertRequest"/> struct containing:
    /// <list type="bullet">
    ///   <item><c>Success</c>: whether to show the check or cross icon.</item>
    ///   <item><c>Message</c>: the text to display.</item>
    ///   <item><c>FadeInTime</c>, <c>DisplayTime</c>, <c>FadeOutTime</c>: timing parameters for the fade sequence.</item>
    /// </list>
    /// </param>
    private void DisplayAlert(AlertRequest req)
    {
        // Instantiate the AlertView prefab under the Alerts parent
        var view = Instantiate(alertPrefab, alertsParent);
        currentActiveAlerts++;

        // Subscribe to completion and start the sequence
        view.OnFadeOutComplete += HandleAlertFinished;
        view.Setup(req.Success, req.Message, req.FadeInTime, req.DisplayTime, req.FadeOutTime);
    }

    /// <summary>
    /// Cleans up a finished alert view and, if there are more alerts queued,
    /// dequeues and displays the next one.
    /// </summary>
    /// <param name="view">
    /// The <see cref="AlertView"/> instance that has completed its fade-out animation.
    /// </param>
    private void HandleAlertFinished(AlertView view)
    {
        // Unsubscribe and destroy the view
        view.OnFadeOutComplete -= HandleAlertFinished;
        Destroy(view.gameObject);
        currentActiveAlerts--;

        // Dequeue and display next alert if any
        if (alertQueue.Count > 0)
        {
            var next = alertQueue.Dequeue();
            DisplayAlert(next);
        }
    }

    /// <summary>
    /// Internal model representing an alert request.
    /// </summary>
    private struct AlertRequest
    {
        public bool Success { get; }
        public string Message { get; }
        public float FadeInTime { get; }
        public float DisplayTime { get; }
        public float FadeOutTime { get; }

        public AlertRequest(bool success, string message, float fadeInTime, float displayTime, float fadeOutTime)
        {
            Success = success;
            Message = message;
            FadeInTime = fadeInTime;
            DisplayTime = displayTime;
            FadeOutTime = fadeOutTime;
        }
    }
}