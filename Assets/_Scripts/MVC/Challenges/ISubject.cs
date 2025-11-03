/// <summary>
/// Defines the interface for subjects that can register, unregister, and notify observers of challenge-related events.
/// </summary>
public interface ISubject
{
    /// <summary>
    /// Subscribes an observer to receive event notifications.
    /// </summary>
    /// <param name="observer">The observer to add.</param>
    void AddObserver(IChallengeObserver observer);

    /// <summary>
    /// Unsubscribes an observer so it no longer receives event notifications.
    /// </summary>
    /// <param name="observer">The observer to remove.</param>
    void RemoveObserver(IChallengeObserver observer);

    /// <summary>
    /// Notifies all registered observers of a specific event.
    /// </summary>
    /// <param name="eventType">The type of event that occurred.</param>
    /// <param name="amount">The numerical value associated with the event.</param>
    void NotifyObservers(EventType eventType, int amount);
}
