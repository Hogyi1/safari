using System.Collections.Generic;
using UnityEngine;

// Time eventek megfigyelése
public class TimeEvents : MonoBehaviour, ITimeEventObservable
{
    // A feliratkozott observerek
    private List<ITimeEventObserver> observers = new List<ITimeEventObserver>();

    // Singleton pattern követése
    public static TimeEvents Instance { get; private set; }
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
    public void AddObserver(ITimeEventObserver observer)
    {
        Debug.Log("Új Observer iratkozott fel a Time Event figyelésére " + observer);

        observers.Add(observer);
    }

    // Értesítés kiküldése
    public void NotifyObservers(TimeEvent timeEvent)
    {
        Debug.Log("Új Time Event érkezett továbbításra " + timeEvent.ToString());

        foreach (ITimeEventObserver observer in observers)
        {
            observer.OnNotify(timeEvent);
        }
    }

    // Leiratkozás
    public void RemoveObserver(ITimeEventObserver observer)
    {
        Debug.Log("Observer leiratkozott a Time Event figyeléséről " + observer);

        observers.Remove(observer);
    }

}

public enum TimeEvent
{
    Day_passed,
    Month_passed,
    Year_passed
}

