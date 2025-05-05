using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

public class Vehicle
{
    public int ID { get; private set; }
    [SerializeField]
    private List<Tourist> AssignedTourists = new List<Tourist>();
    public VehicleState State;
    public VehicleType Type;
    public int Capacity;
    public int SpaceTaken;
    public float WaitingTime = 0f;
    public Vector3 Position;

    public Vehicle(int ID, VehicleData data)
    {
        this.ID = ID;
        this.Capacity = data.capacity;
        this.SpaceTaken = data.spacetaken;
        this.Type = data.type;
        this.State = VehicleState.Empty;
    }

    // True ha mindenki megérkezett és már a kocsiban ül
    public bool AllPassengersArrived()
    {
        return AssignedTourists.FindAll(t => t.GetState() == TouristState.Walking).Count < 1;
    }
    public bool AddPassenger(Tourist tourist)
    {
        if (tourist == null || AssignedTourists.Count >= Capacity)
            return false;

        AssignedTourists.Add(tourist);
        State = AssignedTourists.Count == Capacity ? VehicleState.Full : VehicleState.Waiting;

        Debug.Log("Ennyi ideje várakozom: " + WaitingTime + "| Ennyi túristám van: " + AssignedTourists.Count);

        return true;
    }

    public void SetTouristState(TouristState state)
    {
        foreach (var tourist in AssignedTourists)
        {
            tourist.SetState(state);
        }
    }

    public VehicleState GetState()
    {
        return State;
    }

    public int GetID()
    {
        return ID;
    }

    public void AddWaitingTime(float time)
    {
        WaitingTime += time;
    }

    public void ResetWaitingTime()
    {
        WaitingTime = 0;
    }

    public void UpdateTouristPosition(Vector3 newPosition)
    {
        foreach (var t in AssignedTourists)
        {
            t.Position = newPosition;
        }
    }

    public void ClearPassengers()
    {
        foreach (var t in AssignedTourists)
        {
            t.SetState(TouristState.Finished);
        }

        AssignedTourists.Clear();
    }
}

public enum VehicleType
{
    Jeep,
    Bus,
    Van
}