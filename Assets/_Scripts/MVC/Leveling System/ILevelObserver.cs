/// <summary>
/// Defines a contract for classes that want to observe level-related events.
/// </summary>
/// <param name="eventType">Type of the event being notified.</param>
/// <param name="amount">Associated amount with the event (e.g., experience points).</param>
public interface ILevelObserver
{
    void OnNotify(EventType eventType, int amount);
}
