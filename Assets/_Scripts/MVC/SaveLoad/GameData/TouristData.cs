using UnityEngine;

[System.Serializable]
public class TouristData
{
    public float OverallMood ;
    public float OverallWaitingMood ;
    public bool Incoming;
    public int TouristCount;
    public float moodSensitivity;

    public TouristData()
    {
        OverallMood = 50f;
        OverallWaitingMood = 50f;
        Incoming = false;
        TouristCount = 0;
        moodSensitivity = 0.5f;
    }

}
