using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour, ISubject
{
    private static GameEvents _instance;
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

    private List<IChallengeObserver> observers = new();

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

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    public void AddObserver(IChallengeObserver observer)
    {
        if (!observers.Contains(observer))
        {
            observers.Add(observer);
        }
    }

    public void RemoveObserver(IChallengeObserver observer)
    {
        if (observers.Contains(observer))
        {
            observers.Remove(observer);
        }
    }

    public void NotifyObservers(EventType eventType, int amount)
    {
        foreach (var observer in observers)
        {
            observer.OnNotify(eventType, amount);
        }
    }
}
