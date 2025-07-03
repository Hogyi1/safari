using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a vehicle in the system, managing passenger assignments and waiting time.
/// </summary>
[System.Serializable]
public class VehicleModel
{
    /// <summary>
    /// Unique identifier of the vehicle.
    /// </summary>
    [SerializeField] private int iD;

    /// <summary>
    /// Current operational state of the vehicle.
    /// </summary>
    [SerializeField] private VehicleState state;

    /// <summary>
    /// Type of the vehicle (e.g., Jeep, Bus, Van).
    /// </summary>
    [SerializeField] private VehicleType type;

    /// <summary>
    /// Maximum number of passengers the vehicle can hold.
    /// </summary>
    [SerializeField] private int capacity;

    /// <summary>
    /// Space taken up in garage
    /// </summary>
    [SerializeField] private int spaceTaken;

    /// <summary>
    /// Accumulated waiting time in seconds.
    /// </summary>
    [SerializeField] private float waitingTime;

    /// <summary>
    /// IDs of tourists assigned to this vehicle.
    /// </summary>
    public List<int> AssignedTouristIDs = new List<int>();

    /// <summary>
    /// Gets the unique identifier of the vehicle.
    /// </summary>
    public int ID => iD;

    /// <summary>
    /// Gets or sets the current state of the vehicle.
    /// </summary>
    public VehicleState State
    {
        get => state;
        set => state = value;
    }

    /// <summary>
    /// Gets the type of the vehicle.
    /// </summary>
    public VehicleType Type => type;

    /// <summary>
    /// Gets the maximum capacity of the vehicle.
    /// </summary>
    public int Capacity => capacity;

    /// <summary>
    /// Gets the space taken in the garage
    /// </summary>
    public int SpaceTaken => spaceTaken;

    /// <summary>
    /// Gets the total waiting time accumulated.
    /// </summary>
    public float WaitingTime => waitingTime;

    /// <summary>
    /// Icon of vehicles
    /// </summary>
    public Sprite Icon;


    /// <summary>
    /// Initializes a new instance of the <see cref="VehicleModel"/> class.
    /// </summary>
    /// <param name="id">Unique identifier for this vehicle.</param>
    /// <param name="data">Data object containing type, capacity, and space-taken values.</param>
    public VehicleModel(int id, VehicleData data)
    {
        iD = id;
        type = data.Type;
        capacity = data.Capacity;
        spaceTaken = data.SpaceTaken;
        state = VehicleState.Empty;
        waitingTime = 0f;
        this.Icon = data.Icon;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VehicleModel"/> class.
    /// </summary>
    /// <param name="vehicleData">Unique save for this vehicle.</param>
    /// <param name="data">Data object containing type, capacity, and space-taken values.</param>
    public VehicleModel(VehicleSaveData vehicleData, VehicleData data)
    {
        iD = vehicleData.ID;
        type = data.Type;
        capacity = data.Capacity;
        spaceTaken = data.SpaceTaken;
        state = vehicleData.State;
        waitingTime = vehicleData.WaitingTime;
        AssignedTouristIDs = vehicleData.AssignedTourists;
        Icon = data.Icon;
    }


    /// <summary>
    /// Attempts to add a passenger by their tourist ID.
    /// </summary>
    /// <param name="touristId">ID of the tourist to add.</param>
    /// <returns>True if added; false if capacity is full.</returns>
    public bool AddPassenger(int touristId)
    {
        if (AssignedTouristIDs.Count >= capacity)
            return false;

        AssignedTouristIDs.Add(touristId);
        state = AssignedTouristIDs.Count == capacity
            ? VehicleState.Full
            : VehicleState.Waiting;
        return true;
    }


    /// <summary>
    /// Adds to the waiting time counter.
    /// </summary>
    /// <param name="time">Seconds to add.</param>
    public void AddWaitingTime(float time)
    {
        waitingTime += time;
    }


    /// <summary>
    /// Clears all assigned passengers and resets waiting time.
    /// </summary>
    public void ClearPassengers()
    {
        AssignedTouristIDs.Clear();
        waitingTime = 0f;
    }

}

/// <summary>
/// Types of vehicles available in the system.
/// </summary>
public enum VehicleType
{
    Jeep,
    Bus,
    Van
}
