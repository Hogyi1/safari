using UnityEngine;

public class TestChallengeProgress : MonoBehaviour
{
    public void TestProgress_EARN_MONEY()
    {
        GameEvents.Instance.NotifyObservers(EventType.MONEY_GAIN, 100);
    }
}
