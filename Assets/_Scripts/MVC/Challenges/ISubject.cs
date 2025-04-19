public interface ISubject
{
    void AddObserver(IChallengeObserver observer);
    void RemoveObserver(IChallengeObserver observer);
    void NotifyObservers(EventType eventType, int amount);
}
