using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for objects that can be observed for time events.
/// </summary>
public interface ITimeEventObservable
{
    /// <summary>
    /// Registers an observer to receive time event notifications.
    /// </summary>
    /// <param name="observer">Observer to add.</param>
    public void AddObserver(ITimeEventObserver observer);

    /// <summary>
    /// Unregisters an observer from time event notifications.
    /// </summary>
    /// <param name="observer">Observer to remove.</param>
    public void RemoveObserver(ITimeEventObserver observer);

    /// <summary>
    /// Notifies all registered observers of a time event.
    /// </summary>
    /// <param name="timeEvent">The time event to notify.</param>
    public void NotifyObservers(TimeEvent timeEvent);
}
