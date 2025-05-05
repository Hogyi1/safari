using System.Collections.Generic;
using UnityEngine;

// Random eventek megfigyelése
public class RandomEvents : MonoBehaviour, IRandomEventObservable
{
    // A feliratkozott observerek
    private List<IRandomEventObserver> observers = new List<IRandomEventObserver>();

    // Singleton pattern követése
    public static RandomEvents Instance { get; private set; }
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

    // Feliratkozás
    public void AddObserver(IRandomEventObserver observer)
    {
        Debug.Log("Új Observer iratkozott fel a Random Event figyelésére " + observer);

        observers.Add(observer);
    }

    // Értesítés kiküldése
    public void NotifyObservers(RandomEvent randomEvent)
    {
        // Debug.Log("Új Random Event érkezett továbbításra " + randomEvent.ToString());

        foreach (IRandomEventObserver observer in observers)
        {
            observer.OnNotify(randomEvent);
        }
    }

    // Leiratkozás
    public void RemoveObserver(IRandomEventObserver observer)
    {
        Debug.Log("Observer leiratkozott a Random Event figyeléséről " + observer);

        observers.Remove(observer);
    }
}

public enum RandomEvent
{
    Spawn_tourist,
    Raid,
    Regrow,
    Breed_animal,
    None
}
