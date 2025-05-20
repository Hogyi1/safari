using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static VehicleState;

/// <summary>
/// Controls the lifecycle and routing of vehicles, assigns tourists to vehicles,
/// and manages tour start and finish in an MVC architecture.
/// </summary>
[RequireComponent(typeof(VehicleFactory))]
public class VehicleManager : MonoBehaviour, IUpgradeable, IBuyableManager, IDataPersistence
{
    /// <summary>
    /// Singleton instance of the VehicleManager.
    /// </summary>
    public static VehicleManager Instance { get; private set; }

    [SerializeField] private VehicleFactory factory;

    [Tooltip("Which manager type is mine")]
    [SerializeField] private ManagerType myType = ManagerType.Vehicle;

    [Tooltip("Maximum waiting time per vehicle in seconds")]
    [SerializeField] private const float maxWaitingTime = 15f;

    private List<Vehicle> activeVehicles = new List<Vehicle>();
    private int maxCapacity;
    private Action OnHandlerResponse;

    /// <summary>
    /// Unity lifecycle method that initializes the singleton instance,
    /// subscribes to road-related events, and disables the GameObject by default.
    /// Ensures only one instance of the manager exists across scenes.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        factory = GetComponent<VehicleFactory>();

        // Events
        OnHandlerResponse = () => gameObject.SetActive(true);
        DataPersistenceManager.Instance.OnAllLoaded += OnHandlerResponse;

        gameObject.SetActive(false);
    }

    private void OnDestroy() => DataPersistenceManager.Instance.OnAllLoaded -= OnHandlerResponse;

    private void Start() => RoadManager.Instance.OnRoadRemoved += HandleRedirect;

    /// <summary>
    /// Unity lifecycle method that initializes the singleton instance,
    /// subscribes to road-related events, and disables the GameObject by default.
    /// Ensures only one instance of the manager exists across scenes.
    /// </summary>
    private void Update()
    {
        float delta = Time.deltaTime;

        foreach (var vehicle in activeVehicles)
        {
            if (vehicle.IsUnityNull()) return;
            switch (vehicle.Model.State)
            {
                case Full:
                    StartTour(vehicle.ID);
                    break;
                case Finished:
                    FinishTour(vehicle.ID);
                    break;
                case Waiting:
                    vehicle.Model.AddWaitingTime(delta);
                    if (vehicle.Model.WaitingTime >= maxWaitingTime) StartTour(vehicle.ID);
                    break;
                default:
                    break;
            }
        }
    }


    /// <summary>
    /// Spawns a new vehicle at a parking spot and registers it.
    /// </summary>
    /// <param name="vehicleTypeIndex">Index to select vehicle data from factory.</param>
    public void SpawnVehicle(int vehicleTypeIndex)
    {
        if (maxCapacity <= activeVehicles.Count) return;
        int id = IDGenerator.GenerateID();

        Vehicle newVehicle = factory.CreateVehicle(id, vehicleTypeIndex);
        if (newVehicle != null)
            activeVehicles.Add(newVehicle);
    }


    /// <summary>
    /// Removes a vehicle by ID, destroying its view GameObject if present.
    /// </summary>
    /// <param name="id">Unique identifier of the vehicle to remove.</param>
    public void RemoveVehicle(int id)
    {
        var toRemove = activeVehicles.Find(v => v.ID == id);

        if (toRemove != null)
        {
            toRemove.Model.AssignedTouristIDs
                .ForEach(tid => TouristManager.Instance.SetTouristState(tid, TouristState.Finished));
            activeVehicles.Remove(toRemove);

            if (toRemove.View != null)
                Destroy(toRemove.View.gameObject);
        }
    }


    /// <summary>
    /// Attempts to assign a tourist to an available vehicle.
    /// Returns the door position if successful, otherwise zero vector.
    /// </summary>
    /// <param name="tourist">Tourist to assign.</param>
    /// <returns>Door position of the assigned vehicle or Vector3.zero.</returns>
    public int AssignTouristToVehicle(int touristID)
    {
        if (!RoadManager.Instance.HasRoute)
            return -1;

        var available = activeVehicles
            .Where(v => v.Model.State == Empty || v.Model.State == Waiting)
            .ToList();

        foreach (var vehicle in available)
        {
            if (vehicle.Model.AddPassenger(touristID))
            {
                GameEvents.Instance.NotifyObservers(EventType.VISITOR_TRANSPORTED, 1);
                return vehicle.ID;
            }
        }

        return -1;
    }


    /// <summary>
    /// Finds a random route via the RoadManager.
    /// </summary>
    private List<Vector3> FindRoute() => RoadManager.Instance.SearchForRandomPath();


    /// <summary>
    /// Determines the parking spot position for spawning vehicles.
    /// </summary>
    public Vector3 GetParkingSpot() => FacilityManager.Instance.GetSpawnPosition(myType);


    /// <summary>
    /// Gets the interacting position of the garage
    /// </summary>
    /// <returns>The interacting position of the garage</returns>
    public Vector3 GetGaragePosition() => FacilityManager.Instance.GetInteractingPosition(myType);


    /// <summary>
    /// Checks if all passengers of a vehicle have arrived by querying TouristManager.
    /// </summary>
    private bool AllPassengersArrived(Vehicle vehicle)
    {
        return !vehicle.Model.AssignedTouristIDs
            .Any(id => TouristManager.Instance.GetTouristState(id) == TouristState.Walking);
    }


    /// <summary>
    /// Starts the tour for a vehicle if all passengers have arrived.
    /// </summary>
    /// <param name="id">Vehicle ID to start tour.</param>
    private void StartTour(int id)
    {
        var vehicle = activeVehicles.Find(v => v.ID == id);

        if (vehicle != null && AllPassengersArrived(vehicle))
        {
            vehicle.Model.State = On_tour;
            vehicle.Model.AssignedTouristIDs
                .ForEach(tid => TouristManager.Instance.SetTouristState(tid, TouristState.On_tour));

            vehicle.View.gameObject.SetActive(true);
            vehicle.View.MoveOnRoute(FindRoute(), Finished);
            GameEvents.Instance.NotifyObservers(EventType.EXP_ADD, 5);
        }
    }


    /// <summary>
    /// Finishes the tour for a vehicle, resets passenger states, and returns it to parking.
    /// </summary>
    /// <param name="id">Vehicle ID to finish tour.</param>
    private void FinishTour(int id)
    {
        var vehicle = activeVehicles.Find(v => v.ID == id);

        if (vehicle != null)
        {
            vehicle.Model.AssignedTouristIDs
                .ForEach(tid => TouristManager.Instance.SetTouristState(tid, TouristState.Finished));

            List<Vector3> backRoute = RoadManager.Instance.FindNewPath(vehicle.View.transform.position, false);
            if (backRoute.Count == 0) vehicle.View.ResetVehicle();
            else vehicle.View.MoveOnRoute(backRoute, Empty);

            vehicle.Model.State = Busy;
            vehicle.Model.ClearPassengers();
        }
    }


    /// <summary>
    /// Handles rerouting of all vehicles currently on tour or returning (busy) when a road is removed.
    /// Attempts to find a new valid path using the <see cref="RoadManager"/>.
    /// - If no path is found and the vehicle was on tour, it is marked as <c>Finished</c>.
    /// - If no path is found and the vehicle was returning, it is reset to garage.
    /// - If a path is found, the vehicle is redirected using <see cref="VehicleView.MoveOnRoute"/>.
    /// </summary>
    private void HandleRedirect()
    {
        var vehiclesOnTour = activeVehicles.FindAll(t => t.Model.State == On_tour || t.Model.State == Busy);

        foreach (var vehicle in vehiclesOnTour)
        {
            bool toExit = vehicle.Model.State == On_tour;
            List<Vector3> newPath = RoadManager.Instance.FindNewPath(vehicle.View.transform.position, toExit);
            vehicle.View.StopAllCoroutines();

            if (newPath.Count == 0 && toExit) vehicle.Model.State = Finished;
            else if (newPath.Count == 0 && !toExit) vehicle.View.ResetVehicle();
            else if (toExit) vehicle.View.MoveOnRoute(newPath, Finished);
            else vehicle.View.MoveOnRoute(newPath, Empty);
        }
    }


    /* GETTERS SETTERS */
    public void SetVehicleState(int iD, VehicleState newState) => activeVehicles.Find(t => t.ID == iD).Model.State = newState;
    public Vehicle GetVehicle(int ID) => activeVehicles.Find(t => t.ID == ID);
    public List<AnimalType> GetAnimalsInSight(int vehicleID) => activeVehicles.FirstOrDefault(t => t.ID == vehicleID).View.AnimalsInView;
    public void LevelUp(int amount) => maxCapacity += amount;
    public void LevelDown(int amount) => maxCapacity -= amount;
    public bool CanBuy() => Capacity < MaxCapacity;
    public void SetCapacity(int amount) => maxCapacity = amount;
    public int MaxCapacity => maxCapacity;
    public int Capacity => activeVehicles.Count;
    public List<Vehicle> GetAllVehicles() => activeVehicles;


    /// <summary>
    /// Gets the loading priority for this manager. Lower values are loaded earlier.
    /// Used to ensure vehicles are loaded after essential dependencies like roads.
    /// </summary>
    public float Priority => 1000f;


    /// <summary>
    /// Loads vehicle-related data from the provided <see cref="GameData"/> object.
    /// Restores the maximum vehicle capacity and re-creates all saved vehicles.
    /// </summary>
    /// <param name="data">The <see cref="GameData"/> object containing saved vehicle data.</param>
    /// <returns>Coroutine enumerator used for asynchronous data loading.</returns>
    public IEnumerator LoadData(GameData data)
    {
        yield return Register(data.vehicleDatas);
    }


    /// <summary>
    /// Saves the current state of all vehicles to the provided <see cref="GameData"/> object.
    /// Includes both manager-level data (e.g., max capacity) and individual vehicle states.
    /// </summary>
    /// <param name="data">The <see cref="GameData"/> object to write vehicle data into.</param>
    public void SaveData(GameData data)
    {
        data.vehicleDatas.Clear();
        activeVehicles.ForEach(t => data.vehicleDatas.Add(t.GetSaveData()));
    }


    /// <summary>
    /// Asynchronously registers all vehicles from a saved list by re-creating them through the factory.
    /// Waits one frame before beginning to ensure dependencies (like scene setup) are ready.
    /// </summary>
    /// <param name="data">List of <see cref="VehicleSaveData"/> entries to restore.</param>
    /// <returns>Coroutine that completes after all vehicles have been instantiated and added.</returns>
    private IEnumerator Register(List<VehicleSaveData> data)
    {
        yield return new WaitForEndOfFrame();
        Debug.Log("Elkezdtem az autokat visszatölteni");
        Debug.Log("Factory null?" + factory.IsUnityNull());
        data.ForEach(t => activeVehicles.Add(factory.CreateVehicle(t)));
        yield return new WaitForEndOfFrame();
    }
}

/// <summary>
/// Enumerates possible states of a vehicle's lifecycle.
/// </summary>
public enum VehicleState
{
    /// <summary>No passengers and idle.</summary>
    Empty,

    /// <summary>Waiting for more passengers.</summary>
    Waiting,

    /// <summary>Full and ready to depart.</summary>
    Full,

    /// <summary>Currently on tour with passengers.</summary>
    On_tour,

    /// <summary>Tour completed and returning.</summary>
    Finished,

    /// <summary>Busy after finishing tour awaiting clearance.</summary>
    Busy
}