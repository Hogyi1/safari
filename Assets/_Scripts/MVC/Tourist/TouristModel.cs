using System;
using System.Collections.Generic;
using static TouristState;
using UnityEngine;

/// <summary>
/// Represents the data and logic for a single tourist in the MVC pattern.
/// Manages state transitions and mood calculations based on waiting time and animals seen.
/// </summary>
public class TouristModel
{
    // Backing fields
    private readonly int iD;
    private TouristState state;
    private float waitingMood;
    private float tourMood;
    private float totalMood;
    private float elapsedTime;
    private readonly float patienceLevel;
    private int vehicleID;
    private readonly AnimalType favouriteAnimal;

    /// <summary>
    /// Gets the unique identifier of this tourist.
    /// </summary>
    public int ID => iD;

    /// <summary>
    /// Gets the current state of the tourist (e.g., In_queue, On_tour).
    /// </summary>
    public TouristState State => state;

    /// <summary>
    /// Gets the current mood value while waiting. Range [0.01, 100].
    /// </summary>
    public float WaitingMood => waitingMood;

    /// <summary>
    /// Gets the current mood value during the tour. Range [0.01, 100].
    /// </summary>
    public float TourMood => tourMood;

    /// <summary>
    /// Gets the combined total mood value. Weighted sum of waiting and tour moods.
    /// </summary>
    public float TotalMood => totalMood;

    /// <summary>
    /// Gets the elapsed time (in seconds) used for mood calculations.
    /// </summary>
    public float ElapsedTime => elapsedTime;

    /// <summary>
    /// Gets the type of animal this tourist prefers.
    /// </summary>
    public AnimalType FavouriteAnimalType => favouriteAnimal;

    /// <summary>
    /// Gets, sets the ID of the vehicle the tourist is assigned to.
    /// </summary>
    public int VehicleID
    {
        get => vehicleID;
        set => vehicleID = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TouristModel"/> class.
    /// </summary>
    /// <param name="id">Unique identifier for the tourist.</param>
    public TouristModel(int id)
    {
        iD = id;
        patienceLevel = UnityEngine.Random.Range(-0.03f, 0f);

        int enumLength = Enum.GetValues(typeof(AnimalType)).Length;
        favouriteAnimal = (AnimalType)UnityEngine.Random.Range(0, enumLength);

        elapsedTime = 0f;
        state = In_queue;
        waitingMood = 100f;
        tourMood = 50f;
        vehicleID = -1;
    }


    /// <summary>
    /// Sets the state of the tourist.
    /// </summary>
    /// <param name="newState">The new state to assign.</param>
    public void SetState(TouristState newState)
    {
        state = newState;
    }

    /// <summary>
    /// Adds time (in seconds) to the internal elapsed timer for mood calculation.
    /// </summary>
    /// <param name="delta">Time to add, in seconds.</param>
    public void AddElapsedTime(float delta)
    {
        elapsedTime += delta;
    }

    /// <summary>
    /// Resets the internal elapsed timer to a specified value.
    /// </summary>
    /// <param name="time">New elapsed time, in seconds.</param>
    public void SetElapsedTime(float time)
    {
        elapsedTime = time;
    }

    /// <summary>
    /// Calculates the tourist's current mood based on waiting and animals seen.
    /// Updates internal mood fields and returns the weighted total mood.
    /// </summary>
    /// <param name="animals">List of animals currently seen by this tourist.</param>
    /// <returns>The combined mood value.</returns>
    public void CalculateMood(List<AnimalType> animals)
    {
        int animalsSeen = animals.Count;
        bool hasFavourite = animals.Contains(favouriteAnimal);

        float waiting = CalculateWaitingMood();
        float touring = CalculateTourMood(animalsSeen, hasFavourite);
    }

    /// <summary>
    /// Calculates waiting mood based on exponential decay formula.
    /// </summary>
    private float CalculateWaitingMood()
    {
        if (state == In_queue)
        {
            waitingMood = 100f * Mathf.Exp(patienceLevel * elapsedTime);

            if (waitingMood < 15f)
            {
                waitingMood = 0f;
                tourMood = 0f;
                state = Finished;
            }
        }

        return waitingMood;
    }

    /// <summary>
    /// Calculates tour mood based on animals seen and presence of favourite species.
    /// </summary>
    /// <param name="animalsSeen">Number of animals seen.</param>
    /// <param name="hasFavourite">Whether any seen animal matches favourite type.</param>
    private float CalculateTourMood(int animalsSeen, bool hasFavourite)
    {
        if (state == On_tour)
        {
            float factor = hasFavourite ? 5f : 2f;
            float deltaMood = (animalsSeen > 0) ? animalsSeen * factor : -2f;

            tourMood = Mathf.Clamp(tourMood + deltaMood * elapsedTime, 0.01f, 100f);
        }

        return tourMood;
    }
}