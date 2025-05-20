using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores all relevant data about a vehicle for saving and loading its state.
/// </summary>
[System.Serializable]
public class VehicleSaveData
{
    /// <summary>
    /// Unique identifier of the vehicle.
    /// </summary>
    public int ID;

    /// <summary>
    /// The type of the vehicle (e.g., jeep, boat).
    /// </summary>
    public VehicleType Type;

    /// <summary>
    /// Maximum number of tourists the vehicle can hold.
    /// </summary>
    public int Capacity;

    /// <summary>
    /// Amount of space the vehicle occupies on the map.
    /// </summary>
    public int SpaceTaken;

    /// <summary>
    /// Time the vehicle has been waiting (e.g., at a station).
    /// </summary>
    public float WaitingTime;

    /// <summary>
    /// List of tourist IDs currently assigned to the vehicle.
    /// </summary>
    public List<int> AssignedTourists = new();

    /// <summary>
    /// Current operational state of the vehicle.
    /// </summary>
    public VehicleState State;

    /// <summary>
    /// The current route that the vehicle is following, represented as a list of world positions.
    /// </summary>
    public List<Vector3> CurrentRoute;

    /// <summary>
    /// The upcoming state the vehicle is transitioning into.
    /// </summary>
    public VehicleState NextState;

    /// <summary>
    /// The current world position of the vehicle.
    /// </summary>
    public Vector3 CurrentPosition;

    /// <summary>
    /// List of animal types that are currently visible from the vehicle.
    /// </summary>
    public List<AnimalType> AnimalsInView = new();
}
