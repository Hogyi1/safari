using UnityEngine;

/// <summary>
/// Observer interface for receiving time event notifications.
/// </summary>
public interface ITimeEventObserver
{
    /// <summary>
    /// Called when a time event occurs.
    /// </summary>
    /// <param name="timeEvent">The event that occurred.</param>
    public void OnNotify(TimeEvent timeEvent);
}
