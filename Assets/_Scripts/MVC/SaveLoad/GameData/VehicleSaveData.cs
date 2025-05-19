using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VehicleSaveData
{
    public int ID;
    public VehicleType Type;
    public int Capacity;
    public int SpaceTaken;
    public float WaitingTime;
    public List<int> AssignedTourists = new();
    public VehicleState State;
    public List<Vector3> CurrentRoute;
    public VehicleState NextState;
    public Vector3 CurrentPosition;
    public List<AnimalType> AnimalsInView = new();
}