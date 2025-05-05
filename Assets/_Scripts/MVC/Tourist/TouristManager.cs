using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class TouristManager : MonoBehaviour, IRandomEventObserver
{
    // Singleton pattern
    public static TouristManager Instance { get; private set; }

    // Az aktív touristok
    private List<Tourist> activeTourists = new List<Tourist>();
    private Dictionary<int, TouristView> touristViews = new Dictionary<int, TouristView>();

    private TouristFactory factory;

    // Az átlag kedv
    private float OverallMood = 0f;


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
        factory = GetComponentInChildren<TouristFactory>();
        RandomEvents.Instance.AddObserver(this);
    }

    public void SpawnTourist()
    {

        Tourist newTourist = new Tourist(IDGenerator.GenerateID());
        activeTourists.Add(newTourist);

        //GameObject newTouristGO = Instantiate(touristPrefab, Entrance, Quaternion.identity);
        //TouristView view = newTouristGO.GetComponent<TouristView>();
        //view.Init(newTourist);

        TouristView view = factory.CreateTourist(newTourist);

        touristViews[newTourist.GetID()] = view;
        Debug.Log($"Új turista ID: {newTourist.GetID()}, Patience: {newTourist.patienceLevel}");

    }

    public void RemoveTourist(int touristID)
    {
        Tourist tourist = activeTourists.Find(t => t.GetID() == touristID);
        if (tourist != null)
        {
            activeTourists.Remove(tourist);
        }

        if (touristViews.TryGetValue(touristID, out TouristView view))
        {
            touristViews.Remove(touristID);
            Destroy(view);
        }
    }

    public void Update()
    {
        var Mood = 0f;
        var finishedTourists = activeTourists.Where(t => t.GetState() == TouristState.Finished).ToList();
        foreach (var tourist in finishedTourists)
        {
            RemoveTourist(tourist.GetID());
        }

        foreach (var tourist in activeTourists)
        {
            float delta = Time.deltaTime;
            int AnimalsInRange = 0;
            if (tourist.GetState() == TouristState.In_queue)
            {
                tourist.AddElapsedTime(delta);
                var carPosition = VehicleManager.Instance.AssignTouristToVehicle(tourist);
                if (carPosition != null)
                {
                    TouristView view = touristViews[tourist.GetID()];
                    tourist.SetState(TouristState.Walking);
                    view.StartWalkingToCar((Vector3)carPosition);
                    EconomyManager.Instance.PayForTicket();
                }
            }
            else if (tourist.GetState() == TouristState.On_tour)
            {
                AnimalsInRange = GetAnimalsInRange();
                tourist.SetElapsedTime(delta);
            }

            // Később ide jön az, hogy megnézi mennyi állat van körülötte
            tourist.CalculateMood(AnimalsInRange);

            Mood += tourist.TotalMood;
        }

        OverallMood = Mood / activeTourists.Count;
        // Debug.Log(OverallMood);
    }

    public int GetAnimalsInRange()
    {
        // TODO
        return 0;
    }

    public void OnNotify(RandomEvent randomEvent)
    {

        if (randomEvent == RandomEvent.Spawn_tourist)
        {
            SpawnTourist();
        }
    }
}

public enum TouristState
{
    In_queue,
    Walking,
    In_car,
    On_tour,
    Finished
}
