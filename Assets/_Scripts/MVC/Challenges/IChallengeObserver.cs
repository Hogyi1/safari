/// <summary>
/// Defines the interface for observers that respond to challenge-related events.
/// </summary>
public interface IChallengeObserver
{
    /// <summary>
    /// Called when a subscribed event occurs, providing the event type and associated amount.
    /// </summary>
    /// <param name="eventType">The type of event that occurred.</param>
    /// <param name="amount">The value associated with the event.</param>
    void OnNotify(EventType eventType, int amount);
}
