using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Observable singleton for time-based events. Manages subscriptions and notifies on day, month, year transitions.
/// </summary>
public class TimeEvents : MonoBehaviour, ITimeEventObservable
{
    /// <summary>
    /// List of subscribed time event observers.
    /// </summary>
    private List<ITimeEventObserver> observers = new List<ITimeEventObserver>();

    /// <summary>
    /// Singleton instance of TimeEvents.
    /// </summary>
    public static TimeEvents Instance { get; private set; }

    /// <summary>
    /// Ensures single instance and persists across scenes.
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
    /// Subscribes an observer to time event notifications.
    /// </summary>
    /// <param name="observer">Observer to add.</param>
    public void AddObserver(ITimeEventObserver observer)
    {
        Debug.Log("Új Observer iratkozott fel a Time Event figyelésére " + observer);

        observers.Add(observer);
    }

    /// <summary>
    /// Notifies all observers of a time event.
    /// </summary>
    /// <param name="timeEvent">Time event that occurred.</param>
    public void NotifyObservers(TimeEvent timeEvent)
    {
        Debug.Log("Új Time Event érkezett továbbításra " + timeEvent.ToString());

        foreach (ITimeEventObserver observer in observers)
        {
            observer.OnNotify(timeEvent);
        }
    }

    /// <summary>
    /// Unsubscribes an observer from time event notifications.
    /// </summary>
    /// <param name="observer">Observer to remove.</param>
    public void RemoveObserver(ITimeEventObserver observer)
    {
        Debug.Log("Observer leiratkozott a Time Event figyeléséről " + observer);

        observers.Remove(observer);
    }

}

/// <summary>
/// Types of time events that occur when time advances.
/// </summary>
public enum TimeEvent
{
    Day_passed,
    Month_passed,
    Year_passed
}
