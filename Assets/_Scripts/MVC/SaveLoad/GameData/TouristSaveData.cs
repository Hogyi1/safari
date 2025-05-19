using UnityEngine;

[System.Serializable]
public class TouristSaveData
{
    public int ID;
    public TouristState State;
    public float WaitingMood;
    public float TourMood;
    public float TotalMood;
    public float ElapsedTime;
    public float PatienceLevel;
    public int VehicleID;
    public AnimalType FavouriteAnimal;
    public Vector3 CurrentPosition;
    public Vector3 CurrentDestination;
}
