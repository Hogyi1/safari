using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Observable singleton for random game events. Manages observer subscriptions and notifications.
/// </summary>
public class RandomEvents : MonoBehaviour, IRandomEventObservable
{
    /// <summary>
    /// List of subscribed random event observers.
    /// </summary>
    private List<IRandomEventObserver> observers = new List<IRandomEventObserver>();

    /// <summary>
    /// Singleton instance of RandomEvents.
    /// </summary>
    public static RandomEvents Instance { get; private set; }

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
    /// Subscribes an observer to random event notifications.
    /// </summary>
    /// <param name="observer">Observer to add.</param>
    public void AddObserver(IRandomEventObserver observer)
    {
        Debug.Log("Új Observer iratkozott fel a Random Event figyelésére " + observer);

        observers.Add(observer);
    }

    /// <summary>
    /// Notifies all observers of a random event.
    /// </summary>
    /// <param name="randomEvent">Random event that occurred.</param>
    public void NotifyObservers(RandomEvent randomEvent)
    {
        // Debug.Log("Új Random Event érkezett továbbításra " + randomEvent.ToString());

        foreach (IRandomEventObserver observer in observers)
        {
            observer.OnNotify(randomEvent);
        }
    }

    /// <summary>
    /// Unsubscribes an observer from random event notifications.
    /// </summary>
    /// <param name="observer">Observer to remove.</param>
    public void RemoveObserver(IRandomEventObserver observer)
    {
        Debug.Log("Observer leiratkozott a Random Event figyeléséről " + observer);

        observers.Remove(observer);
    }
}

/// <summary>
/// Types of random events that can occur in the game.
/// </summary>
public enum RandomEvent
{
    Spawn_tourist,
    Raid,
    Regrow,
    Breed_animal,
    None
}
