using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data model representing a vehicle, including capacity, assigned tourists, and state.
/// </summary>
public class Vehicle
{
    /// <summary>
    /// Unique identifier for this vehicle.
    /// </summary>
    public int ID { get; private set; }

    /// <summary>
    /// List of tourists assigned to this vehicle.
    /// </summary>
    [SerializeField]
    private List<Tourist> AssignedTourists = new List<Tourist>();

    /// <summary>
    /// Current state of the vehicle.
    /// </summary>
    public VehicleState State;

    /// <summary>
    /// Type of vehicle (Jeep, Bus, Van).
    /// </summary>
    public VehicleType Type;

    /// <summary>
    /// Maximum number of passengers the vehicle can carry.
    /// </summary>
    public int Capacity;

    /// <summary>
    /// Initial space taken by the vehicle (unused in current logic).
    /// </summary>
    public int SpaceTaken;

    /// <summary>
    /// Accumulated waiting time in seconds.
    /// </summary>
    public float WaitingTime = 0f;

    /// <summary>
    /// Current world position of the vehicle.
    /// </summary>
    public Vector3 Position;

    /// <summary>
    /// Initializes a new Vehicle with an ID and configuration data.
    /// </summary>
    /// <param name="ID">Unique ID to assign.</param>
    /// <param name="data">VehicleData containing capacity and type.</param>
    public Vehicle(int ID, VehicleData data)
    {
        this.ID = ID;
        this.Capacity = data.capacity;
        this.SpaceTaken = data.spacetaken;
        this.Type = data.type;
        this.State = VehicleState.Empty;
    }

    /// <summary>
    /// Checks if all assigned tourists have arrived and boarded.
    /// </summary>
    /// <returns>True if no tourists are still walking.</returns>
    public bool AllPassengersArrived()
    {
        return AssignedTourists.FindAll(t => t.GetState() == TouristState.Walking).Count < 1;
    }

    /// <summary>
    /// Attempts to add a tourist as a passenger and updates state.
    /// </summary>
    /// <param name="tourist">Tourist to add.</param>
    /// <returns>True if added successfully; otherwise false.</returns>
    public bool AddPassenger(Tourist tourist)
    {
        if (tourist == null || AssignedTourists.Count >= Capacity)
            return false;

        AssignedTourists.Add(tourist);
        State = AssignedTourists.Count == Capacity ? VehicleState.Full : VehicleState.Waiting;

        return true;
    }

    /// <summary>
    /// Sets the specified TouristState on all assigned tourists.
    /// </summary>
    /// <param name="state">State to assign to each tourist.</param>
    public void SetTouristState(TouristState state)
    {
        foreach (var tourist in AssignedTourists)
        {
            tourist.SetState(state);
        }
    }

    /// <summary>
    /// Returns the current VehicleState.
    /// </summary>
    /// <returns>Current state.</returns>
    public VehicleState GetState()
    {
        return State;
    }

    /// <summary>
    /// Returns the unique ID of this vehicle.
    /// </summary>
    /// <returns>Vehicle ID.</returns>
    public int GetID()
    {
        return ID;
    }

    /// <summary>
    /// Increments the waiting time by a given amount.
    /// </summary>
    /// <param name="time">Seconds to add.</param>
    public void AddWaitingTime(float time)
    {
        WaitingTime += time;
    }

    /// <summary>
    /// Resets the waiting time to zero.
    /// </summary>
    public void ResetWaitingTime()
    {
        WaitingTime = 0;
    }

    /// <summary>
    /// Updates the position of all assigned tourists to follow the vehicle.
    /// </summary>
    /// <param name="newPosition">New world position.</param>
    public void UpdateTouristPosition(Vector3 newPosition)
    {
        foreach (var t in AssignedTourists)
        {
            t.Position = newPosition;
        }
    }

    /// <summary>
    /// Clears all passengers and marks them as Finished.
    /// </summary>
    public void ClearPassengers()
    {
        foreach (var t in AssignedTourists)
        {
            t.SetState(TouristState.Finished);
        }

        AssignedTourists.Clear();
    }
}

/// <summary>
/// Types of vehicles available in the simulation.
/// </summary>
public enum VehicleType
{
    Jeep,
    Bus,
    Van
}