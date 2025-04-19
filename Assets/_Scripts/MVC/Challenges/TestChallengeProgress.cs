using UnityEngine;

public class TestChallengeProgress : MonoBehaviour
{
    public void TestProgress_EARN_MONEY()
    {
        GameEvents.Instance.NotifyObservers(EventType.EARN_MONEY, 100);
    }
}
