using UnityEngine;
using System;
// Model Layer for Tourist 
public class Tourist
{
    // Adattagok
    public int ID { get; private set; }
    //State ami alapján frissítjük a Moodot
    public TouristState state { get; private set; }
    // Várakozási idő - Mood - Maximum: 100 Minimum: 0.01
    public float WaitingMood { get; private set; }
    // Túrán - Mood - Maximum: 100 Minimum: 0.01
    public float TourMood { get; private set; }
    // Szummája a kettőnek - Mood - Maximum: 100 Minimum: 0.01
    public float TotalMood { get; private set; }
    // Eltelt idő a WaitingMoodhoz
    public float ElapsedTime { get; set; }
    // A türelmességi szintje
    public float patienceLevel;
    // A view éppen hol van
    public Vector3 Position;

    /* 
     * Konstruktor
     * @param ID - Minden túrista egyéni azonosítóval rendelkezik
     */
    public Tourist(int ID)
    {
        System.Random random = new System.Random();

        patienceLevel = (float)(random.NextDouble() * (-0.03f - 0f)) + 0f;
        ElapsedTime = 0;
        state = TouristState.IN_QUEUE;
        TotalMood = 100;
        WaitingMood = 100;
        TourMood = 100;
        this.ID = ID;
    }

    /* 
     * State beállítása 
     * @param newState - az új state
     */
    public void SetState(TouristState newState)
    {
        state = newState;
    }

    /* 
     * Kiszámítja a várakozási, túrázási, illetve a teljes hangulatát
     * @param AnimalsSeen - Egy pozitív szám arról,hogy éppen hány állatot lát a saját körében
     */
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
        }

        // Amennyiben túl sokáig várt hazaküldjük
        if (WaitingMood < 15f)
        {
            TourMood = 0f;
            WaitingMood = 0f;
            state = TouristState.FINISHED;
        }

        TotalMood = (0.3f * WaitingMood) + (0.7f * TourMood);
        // Debug.Log("My mood: " + TotalMood);
    }

    // Visszadja az ID-t
    public int GetID()
    {
        return ID;
    }

    //Visszaadja a state-jét a touristnak
    public TouristState GetState()
    {
        return state;
    }

    // Hozzáadja az eltelt időt
    public void AddElapsedTime(float time)
    {
        ElapsedTime += time;
    }

    // Beállítja az eltelt időt
    public void SetElapsedTime(float time)
    {
        ElapsedTime = time;
    }
}

