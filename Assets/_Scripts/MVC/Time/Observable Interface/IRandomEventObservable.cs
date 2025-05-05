using UnityEngine;

public interface IRandomEventObservable
{
    public void AddObserver(IRandomEventObserver observer);
    public void RemoveObserver(IRandomEventObserver observer);
    public void NotifyObservers(RandomEvent randomEvent);
}
