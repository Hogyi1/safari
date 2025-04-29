using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour, ISubject
{
    private static GameEvents _instance;

    /// <summary>
    /// Singleton instance of the GameEvents class.
    /// </summary>
    public static GameEvents Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<GameEvents>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("GameEvents");
                    _instance = go.AddComponent<GameEvents>();
                }
            }
            return _instance;
        }
    }

    // Observer Lists
    private List<IChallengeObserver> challengeObservers = new();
    private List<ILevelObserver> levelObservers = new();

    /// <summary>
    /// Ensures only one instance of GameEvents exists and persists across scenes.
    /// </summary>
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Resets the singleton instance when this GameObject is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    #region Challenge Observers

    /// <summary>
    /// Adds a challenge observer to the list if not already present.
    /// </summary>
    /// <param name="observer">The challenge observer to add.</param>
    public void AddObserver(IChallengeObserver observer)
    {
        if (!challengeObservers.Contains(observer))
        {
            challengeObservers.Add(observer);
        }
    }

    /// <summary>
    /// Removes a challenge observer from the list if present.
    /// </summary>
    /// <param name="observer">The challenge observer to remove.</param>
    public void RemoveObserver(IChallengeObserver observer)
    {
        if (challengeObservers.Contains(observer))
        {
            challengeObservers.Remove(observer);
        }
    }

    #endregion

    #region Level Observers

    /// <summary>
    /// Adds a level observer to the list if not already present.
    /// </summary>
    /// <param name="observer">The level observer to add.</param>
    public void AddObserver(ILevelObserver observer)
    {
        if (!levelObservers.Contains(observer))
        {
            levelObservers.Add(observer);
        }
    }

    /// <summary>
    /// Removes a level observer from the list if present.
    /// </summary>
    /// <param name="observer">The level observer to remove.</param>
    public void RemoveObserver(ILevelObserver observer)
    {
        if (levelObservers.Contains(observer))
        {
            levelObservers.Remove(observer);
        }
    }

    #endregion

    /// <summary>
    /// Notifies all registered observers about an event.
    /// </summary>
    /// <param name="eventType">The type of event to notify about.</param>
    /// <param name="amount">The amount associated with the event.</param>
    public void NotifyObservers(EventType eventType, int amount)
    {
        // Notify Challenge Observers
        foreach (var observer in challengeObservers)
        {
            observer.OnNotify(eventType, amount);
        }

        // Notify Level Observers
        foreach (var observer in levelObservers)
        {
            observer.OnNotify(eventType, amount);
        }
    }
}
