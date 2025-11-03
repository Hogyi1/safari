using UnityEngine;

/// <summary>
/// Interface for objects that can be observed for random events.
/// </summary>
public interface IRandomEventObservable
{
    /// <summary>
    /// Registers an observer to receive random event notifications.
    /// </summary>
    /// <param name="observer">Observer to add.</param>
    public void AddObserver(IRandomEventObserver observer);

    /// <summary>
    /// Unregisters an observer from random event notifications.
    /// </summary>
    /// <param name="observer">Observer to remove.</param>
    public void RemoveObserver(IRandomEventObserver observer);

    /// <summary>
    /// Notifies all registered observers of a random event.
    /// </summary>
    /// <param name="randomEvent">The random event to notify.</param>
    public void NotifyObservers(RandomEvent randomEvent);
}
