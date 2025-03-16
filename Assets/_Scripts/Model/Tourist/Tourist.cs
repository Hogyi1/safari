using UnityEngine;
using System;
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
    public Vector3 Position { get; set; }
    // Eltelt idő a WaitingMoodhoz
    public float ElapsedTime { get; set; }
    // A türelmességi szintje
    public float patienceLevel;
    public Tourist(Vector3 startingPosition)
    {
        System.Random random = new System.Random();

        patienceLevel = (float)(random.NextDouble() * (-0.03f - 0f)) + 0f;
        ElapsedTime = 0;
        Position = startingPosition;
        state = TouristState.IN_QUEUE;
        TotalMood = 100;
        WaitingMood = 100;
        TourMood = 100;
    }

    public void SetState(TouristState newState)
    {
        state = newState;
    }

    // int AnimalsSeen - Egy pozitív szám éppen, hány állatot lát a körzetében
    public void CalculateMood(int AnimalsSeen)
    {
        switch (state)
        {
            case TouristState.IN_QUEUE:
                // Amennyiben sorban áll, egy 100*e^-0,03*ElapsedTime képlettel kiszámoljuk mennyi legyen a Moodja
                WaitingMood = 100 * Mathf.Exp(patienceLevel * ElapsedTime);
                break;
            case TouristState.ON_TOUR:
                // Az éppen saját körzetben látott állatok alapján kiszámított Mood
                float k = AnimalsSeen > 0 ? AnimalsSeen * 2f : -2f;
                TourMood = Mathf.Clamp(TourMood + k * ElapsedTime, 0.01f, 100);
                break;
            default:
                break;
        }
        TotalMood = (0.3f * WaitingMood) + (0.7f * TourMood);

        if (WaitingMood < 15)
        {
            state = TouristState.FINISHED;
        }
    }
}

