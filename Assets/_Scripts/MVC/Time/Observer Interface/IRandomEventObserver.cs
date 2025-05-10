using UnityEngine;

/// <summary>
/// Observer interface for receiving random event notifications.
/// </summary>
public interface IRandomEventObserver
{
    /// <summary>
    /// Called when a random event occurs.
    /// </summary>
    /// <param name="randomEvent">The event that occurred.</param>
    public void OnNotify(RandomEvent randomEvent);
}
