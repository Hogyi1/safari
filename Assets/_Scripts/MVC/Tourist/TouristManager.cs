using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static TouristState;

/// <summary>
/// Handles the creation, updating, and removal of tourist agents.
/// Manages overall mood statistics and responds to random events.
/// </summary>
[RequireComponent(typeof(TouristFactory))]
public class TouristManager : MonoBehaviour, IRandomEventObserver, IDataPersistence
{
    // Singleton reference
    public static TouristManager Instance { get; private set; }

    // Currently active tourists
    private List<Tourist> activeTourists = new List<Tourist>();

    // Factory responsible for spawning tourists
    [SerializeField] private TouristFactory factory;

    // Global mood statistics
    public float OverallMood = 50f;
    public float OverallWaitingMood = 50f;
    public bool Incoming;

    // Most popular animal type among tourists
    public AnimalType FavouriteAnimal => animalChart.Count == 0 ? default : animalChart.OrderByDescending(kvp => kvp.Value).Last().Key;

    // Total visitors across all time
    public int AllTimeVisitors;

    // Combined metric for mood evaluation based on current tour and waiting states
    public float OverallFeeMood => (OverallMood * 0.7f + OverallWaitingMood * 0.3f) / 2;

    // Current number of tourists
    public int Count => activeTourists.Count;

    // Load priority for save system
    public float Priority => 750f;

    // Mood change multiplier
    [SerializeField] private float moodSensitivity;

    // Tracks which animals are most liked
    private Dictionary<AnimalType, int> animalChart = new();

    private Action OnHandlerResponse;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Subscribe to random event system and defer enabling until data is loaded
        RandomEvents.Instance.AddObserver(this);
        OnHandlerResponse = () => gameObject.SetActive(true);
        DataPersistenceManager.Instance.OnAllLoaded += OnHandlerResponse;

        gameObject.SetActive(false);
    }

    private void OnDestroy() => DataPersistenceManager.Instance.OnAllLoaded -= OnHandlerResponse;

    private void Update()
    {
        List<AnimalType> AnimalsInRange = new();

        // Clean up finished tourists
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

                    // Try to assign to a vehicle
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
            }

            tourist.Model.CalculateMood(AnimalsInRange);
        }
    }


    /// <summary>
    /// Spawns a new tourist and updates global statistics.
    /// </summary>
    public void SpawnTourist()
    {
        int ID = IDGenerator.GenerateID();
        Tourist newTourist = factory.CreateTourist(ID);

        if (newTourist == null) return;

        AllTimeVisitors++;
        activeTourists.Add(newTourist);

        AnimalType key = newTourist.Model.FavouriteAnimalType;
        if (animalChart.ContainsKey(key))
            animalChart[key]++;
        else
            animalChart[key] = 1;

        Incoming = true;
    }


    /// <summary>
    /// Removes a tourist by ID and cleans up the view.
    /// </summary>
    public void RemoveTourist(int touristID)
    {
        Tourist toRemove = activeTourists.Find(t => t.ID == touristID);
        if (toRemove != null)
        {
            Incoming = false;
            activeTourists.Remove(toRemove);
            Destroy(toRemove.View.gameObject);
        }
    }


    /// <summary>
    /// Begins the walking animation and ticket deduction for a tourist.
    /// </summary>
    private void StartTouristWalk(int carID, Tourist tourist)
    {
        tourist.Model.VehicleID = carID;
        tourist.Model.SetState(Walking);
        tourist.View.StartWalkingToCar(VehicleManager.Instance.GetGaragePosition());
        EconomyManager.Instance.PayForTicket();
    }


    /// <summary>
    /// Retrieves the animal types visible to a given vehicle.
    /// </summary>
    public List<AnimalType> GetAnimalsInRange(int vehicleID)
    {
        return VehicleManager.Instance.GetAnimalsInSight(vehicleID);
    }


    /// <summary>
    /// Called when a random event occurs.
    /// </summary>
    public void OnNotify(RandomEvent randomEvent)
    {
        if (randomEvent == RandomEvent.Spawn_tourist)
        {
            float chance = UnityEngine.Random.Range(0f, 1f);
            if (chance <= (OverallMood / 100f) * EconomyManager.Instance.GetTicketInfluence())
                SpawnTourist();
        }
    }

#warning EZ ITT NEM JO
    /// <summary>
    /// Manually set a tourist's state.
    /// </summary>
    public void SetTouristState(int id, TouristState state)
    {
        try { activeTourists.FirstOrDefault(t => t.ID == id).Model.SetState(state); }
        catch { }
    }



    /// <summary>
    /// Get the current state of a tourist by ID.
    /// </summary>
    public TouristState GetTouristState(int id)
    {
        try
        {
            return activeTourists.FirstOrDefault(t => t.ID == id).Model.State;
        }
        catch { }
        return In_car;
    }

    // === Save system integration ===
    public IEnumerator LoadData(GameData data)
    {
        OverallMood = data.touristManagerSaveData.OverallMood;
        OverallWaitingMood = data.touristManagerSaveData.OverallWaitingMood;
        Incoming = data.touristManagerSaveData.Incoming;
        moodSensitivity = data.touristManagerSaveData.MoodSensitivity;

        yield return Register(data.touristDatas);
    }

    public void SaveData(GameData data)
    {
        data.touristManagerSaveData.OverallMood = OverallMood;
        data.touristManagerSaveData.OverallWaitingMood = OverallWaitingMood;
        data.touristManagerSaveData.Incoming = Incoming;
        data.touristManagerSaveData.TouristCount = Count;
        data.touristManagerSaveData.MoodSensitivity = moodSensitivity;

        data.touristDatas.Clear();
        activeTourists.ForEach(t => data.touristDatas.Add(t.GetSaveData()));
    }

    /// <summary>
    /// Registers restored tourists from save data.
    /// </summary>
    private IEnumerator Register(List<TouristSaveData> data)
    {
        yield return new WaitForEndOfFrame();
        data.ForEach(t => activeTourists.Add(factory.CreateTourist(t)));
    }
}

/// <summary>
/// State machine enum for tourist behavior.
/// </summary>
public enum TouristState
{
    In_queue,
    Walking,
    In_car,
    On_tour,
    Finished
}
