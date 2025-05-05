using UnityEngine;

public class TestLevelingSystem : MonoBehaviour
{
    public void Test_EXP_GAIN()
    {
        GameEvents.Instance.NotifyObservers(EventType.EXP_GAIN, 10);
    }
}
