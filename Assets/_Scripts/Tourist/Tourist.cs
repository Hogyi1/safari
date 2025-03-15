using UnityEngine;

// Model Layer for Tourist 
public class Tourist
{
    // Adattagok
    //State ami alapján frissítjük a Moodot
    public TouristState state { get; private set; }
    // Várakozási idő - Mood - Maximum: 100 Minimum: 0.01
    public float WaitingMood { get; private set; }
    // Túrán - Mood - Maximum: 100 Minimum: 0.01
    public float TourMood { get; private set; }
    // Szummája a kettőnek - Mood - Maximum: 100 Minimum: 0.01
    public float TotalMood { get; private set; }
    // Látható-e a Viewban
    public bool IsVisible;
    // A Túrista pozíciója a pályán
    public Vector3 Position { get; set; }
    // Eltelt idő a WaitingMoodhoz
    public float WaitingTime { get; set; };

    public Tourist(Vector3 startingPosition)
    {
        WaitingTime = 0;
        Position = startingPosition;
        state = TouristState.IN_QUEUE;
        TotalMood = 100;
        WaitingMood = 100;
        TourMood = 100;
        IsVisible = true;
    }

    public void SetState(TouristState newState)
    {
        state = newState;
    }



}

