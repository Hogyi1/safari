using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static TouristState;

[RequireComponent(typeof(TouristFactory))]
public class TouristManager : MonoBehaviour, IRandomEventObserver
{
    // Singleton pattern
    public static TouristManager Instance { get; private set; }

    // Az aktív touristok
    private List<Tourist> activeTourists = new List<Tourist>();

    // factory
    [SerializeField] private TouristFactory factory;

    // Az átlag kedv
    public float OverallMood = 50f;
    public float OverallWaitingMood = 50f;
    [SerializeField] private float moodSensitivity = 0.5f;

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {
        RandomEvents.Instance.AddObserver(this);
    }

    public void SpawnTourist()
    {
        if (activeTourists.Count > 0) return;

        int ID = IDGenerator.GenerateID();
        Tourist newTourist = factory.CreateTourist(ID);

        if (newTourist != null)
            activeTourists.Add(newTourist);
    }

    public void RemoveTourist(int touristID)
    {
        Tourist toRemove = activeTourists.Find(t => t.ID == touristID);
        if (toRemove != null)
        {
            activeTourists.Remove(toRemove);
            Destroy(toRemove.View.gameObject);
        }
        Debug.Log("Removed");
    }

    public void Update()
    {
        List<AnimalType> AnimalsInRange = new();
        activeTourists.Where(t => t.Model.State == Finished).ToList().ForEach(t => RemoveTourist(t.ID));

        foreach (var tourist in activeTourists)
        {
            float delta = Time.deltaTime;
            switch (tourist.Model.State)
            {
                case In_queue:
                    float deltaWaitingMood = tourist.Model.WaitingMood - 50f;
                    OverallWaitingMood += deltaWaitingMood * delta * moodSensitivity;
                    OverallWaitingMood = Mathf.Clamp(OverallWaitingMood, 0f, 100f);

                    var carID = VehicleManager.Instance.AssignTouristToVehicle(tourist.ID);
                    if (carID >= 0) StartTouristWalk(carID, tourist);

                    tourist.Model.AddElapsedTime(delta);
                    break;
                case On_tour:
                    float deltaMood = tourist.Model.TourMood - 50f;
                    OverallMood += deltaMood * delta * moodSensitivity;
                    OverallMood = Mathf.Clamp(OverallMood, 0f, 100f);

                    AnimalsInRange = GetAnimalsInRange(tourist.Model.VehicleID);
                    tourist.Model.SetElapsedTime(delta);
                    break;
                default:
                    break;
            }
            tourist.Model.CalculateMood(AnimalsInRange);
        }
    }

    private void StartTouristWalk(int carID, Tourist tourist)
    {
        tourist.Model.VehicleID = carID;

        tourist.Model.SetState(Walking);
        tourist.View.StartWalkingToCar(VehicleManager.Instance.GetGaragePosition(carID));
        EconomyManager.Instance.PayForTicket();
    }

    public List<AnimalType> GetAnimalsInRange(int vehicleID)
    {
        return VehicleManager.Instance.GetAnimalsInSight(vehicleID);
    }

    public void OnNotify(RandomEvent randomEvent)
    {
        if (randomEvent == RandomEvent.Spawn_tourist) // Minél jobb a kedv annál esélyesebb, hogy jönnek
        {
            Debug.Log("Spawn tourist Event");
            float chance = UnityEngine.Random.Range(0f, 1f);
            if (chance <= (OverallMood / 100f)) SpawnTourist();
        }
    }

    public void SetTouristState(int id, TouristState state) =>
        activeTourists.FirstOrDefault(t => t.ID == id).Model.SetState(state);

    public TouristState GetTouristState(int id) =>
        activeTourists.FirstOrDefault(t => t.ID == id).Model.State;
}

public enum TouristState
{
    In_queue,
    Walking,
    In_car,
    On_tour,
    Finished
}
