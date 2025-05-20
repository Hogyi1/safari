using UnityEngine;

/// <summary>
/// Stores all relevant data about a tourist for saving and loading purposes.
/// </summary>
[System.Serializable]
public class TouristSaveData
{
    /// <summary>
    /// Unique identifier of the tourist.
    /// </summary>
    public int ID;

    /// <summary>
    /// The current state of the tourist (e.g., waiting, touring).
    /// </summary>
    public TouristState State;

    /// <summary>
    /// The mood level of the tourist while waiting.
    /// </summary>
    public float WaitingMood;

    /// <summary>
    /// The mood level of the tourist during the tour.
    /// </summary>
    public float TourMood;

    /// <summary>
    /// The total mood score accumulated by the tourist.
    /// </summary>
    public float TotalMood;

    /// <summary>
    /// Total elapsed time the tourist has been active.
    /// </summary>
    public float ElapsedTime;

    /// <summary>
    /// The level of patience the tourist has before becoming dissatisfied.
    /// </summary>
    public float PatienceLevel;

    /// <summary>
    /// The ID of the vehicle the tourist is currently assigned to.
    /// </summary>
    public int VehicleID;

    /// <summary>
    /// The tourist's favorite animal type.
    /// </summary>
    public AnimalType FavouriteAnimal;

    /// <summary>
    /// The tourist's current world position.
    /// </summary>
    public Vector3 CurrentPosition;

    /// <summary>
    /// The tourist's current destination position in the world.
    /// </summary>
    public Vector3 CurrentDestination;
}
