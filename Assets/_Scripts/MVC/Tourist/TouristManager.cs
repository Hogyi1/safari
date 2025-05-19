using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static TouristState;

[RequireComponent(typeof(TouristFactory))]
public class TouristManager : MonoBehaviour, IRandomEventObserver, IDataPersistence
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
    public bool Incoming;

    public AnimalType FavouriteAnimal => animalChart.Count == 0 ? default : animalChart.OrderByDescending(kvp => kvp.Value).Last().Key;
    public int AllTimeVisitors;
    public float OverallFeeMood => (OverallMood * 0.7f + OverallWaitingMood * 0.3f) / 2;
    public int Count => activeTourists.Count;

    [SerializeField] private float moodSensitivity;

    private Dictionary<AnimalType, int> animalChart = new();

    public bool IsLoaded = false;

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        DataPersistenceManager.Instance.OnAllLoaded += AllLoaded;
    }

    public void Start()
    {
        RandomEvents.Instance.AddObserver(this);
    }

    private void AllLoaded() => IsLoaded = true;

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

    public void Update()
    {
        if (!IsLoaded) return;

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
            if (chance <= (OverallMood / 100f) * EconomyManager.Instance.GetTicketInfluence()) SpawnTourist();
        }
    }

    public void SetTouristState(int id, TouristState state) =>
        activeTourists.FirstOrDefault(t => t.ID == id).Model.SetState(state);

    public TouristState GetTouristState(int id) =>
        activeTourists.FirstOrDefault(t => t.ID == id).Model.State;

    public void LoadData(GameData data)
    {
        OverallMood = data.touristManagerSaveData.OverallMood;
        OverallWaitingMood = data.touristManagerSaveData.OverallWaitingMood;
        Incoming = data.touristManagerSaveData.Incoming;
        moodSensitivity = data.touristManagerSaveData.MoodSensitivity;

        StartCoroutine(Register(data.touristDatas));
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
    private IEnumerator Register(List<TouristSaveData> data)
    {
        yield return new WaitForEndOfFrame();
        data.ForEach(t => activeTourists.Add(factory.CreateTourist(t)));
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
