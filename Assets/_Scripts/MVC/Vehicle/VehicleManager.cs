using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Manages vehicle lifecycle: spawning, assignment of tourists, tours, and cleanup.
/// Implements a singleton pattern for global access.
/// </summary>
public class VehicleManager : MonoBehaviour
{
    /// <summary>
    /// Database of all available VehicleData assets loaded from Resources.
    /// </summary>
    [SerializeField]
    public static List<VehicleData> vehicleDatabase = new List<VehicleData>();

    /// <summary>
    /// List of active Vehicle models currently in the simulation.
    /// </summary>
    private List<Vehicle> activeVehicles = new List<Vehicle>();

    /// <summary>
    /// Mapping from vehicle ID to its VehicleView instance.
    /// </summary>
    private Dictionary<int, VehicleView> vehicleViews = new Dictionary<int, VehicleView>();

    /// <summary>
    /// Maximum real-time seconds a vehicle will wait for passengers.
    /// </summary>
    [SerializeField]
    private readonly float MAX_WAITING_TIME = 15f;

    /// <summary>
    /// Maximum number of vehicles allowed simultaneously.
    /// </summary>
    public int capacity = 5;

    /// <summary>
    /// Singleton instance of the VehicleManager.
    /// </summary>
    public static VehicleManager Instance { get; private set; }

    /// <summary>
    /// Ensures a single instance and persists across scene loads.
    /// </summary>
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

    /// <summary>
    /// Loads the vehicle database and spawns an initial Jeep on start.
    /// </summary>
    void Start()
    {
        LoadAllVehicles();
        SpawnVehicle(VehicleType.Jeep);
    }

    /// <summary>
    /// Updates vehicle states each frame, handling tours, waiting, and completion.
    /// </summary>
    void Update()
    {
        foreach (var vehicle in activeVehicles)
        {
            float delta = Time.deltaTime;

            switch (vehicle.GetState())
            {
                case VehicleState.On_tour:
                    // This may not be the most efficient approach; the Tourists themselves could
                    // potentially check the vehicle’s current position instead
                    vehicle.UpdateTouristPosition(vehicleViews[vehicle.GetID()].transform.position);
                    break;
                case VehicleState.Full:
                    StartTour(vehicle.GetID());
                    break;
                case VehicleState.Finished:
                    FinishTour(vehicle.GetID());
                    break;
                case VehicleState.Waiting:
                    vehicle.AddWaitingTime(delta);
                    if (vehicle.WaitingTime >= MAX_WAITING_TIME) StartTour(vehicle.GetID());
                    break;
            }
        }
    }

    /// <summary>
    /// Spawns a new vehicle of the specified type and initializes its view.
    /// </summary>
    /// <param name="Type">Type of vehicle to instantiate.</param>
    public void SpawnVehicle(VehicleType Type)
    {
        var selectedData = vehicleDatabase.Find(t => t.type == Type);
        Vehicle newVehicle = new Vehicle(IDGenerator.GenerateID(), selectedData);
        activeVehicles.Add(newVehicle);
        GameObject newVehicleGO = Instantiate(selectedData.vehiclePrefab, AssignVehicleToParkingSpot(), Quaternion.identity);
        VehicleView view = newVehicleGO.GetComponent<VehicleView>();
        view.Init(newVehicle);
        vehicleViews[newVehicle.GetID()] = view;
    }

    /// <summary>
    /// Removes a vehicle and its view by ID.
    /// </summary>
    /// <param name="vehicleID">ID of the vehicle to remove.</param>
    public void RemoveVehicle(int vehicleID)
    {
        Vehicle vehicle = activeVehicles.Find(t => t.GetID() == vehicleID);
        if (vehicle != null)
        {
            activeVehicles.Remove(vehicle);
        }

        if (vehicleViews.TryGetValue(vehicleID, out VehicleView view))
        {
            vehicleViews.Remove(vehicleID);
            Destroy(view.gameObject);
            Destroy(view);
        }
    }

    /// <summary>
    /// Assigns a tourist to the first available vehicle and returns its door position.
    /// </summary>
    /// <param name="tourist">Tourist to assign.</param>
    /// <returns>Door position for pickup, or null if none available.</returns>
    public Vector3? AssignTouristToVehicle(Tourist tourist)
    {
        if (FindRoute().IsUnityNull()) return null;
        var filteredVehicles = activeVehicles.FindAll(t => t.GetState() != VehicleState.On_tour && t.GetState() != VehicleState.Finished && t.GetState() != VehicleState.Busy);

        foreach (var vehicle in filteredVehicles)
        {
            if (vehicle.AddPassenger(tourist))
            {
                var view = vehicleViews[vehicle.GetID()];
                return view.GetDoorPosition();
            }
        }

        return null;
    }

    /// <summary>
    /// Returns a parking spot position for vehicles when idle.
    /// </summary>
    /// <returns>World position of the parking spot.</returns>
    public Vector3 AssignVehicleToParkingSpot()
    {
        // TODO: Implement the Map/ParkingManager here later
        return new Vector3(39f, 1.05f, 8.0f);
    }

    /// <summary>
    /// Finds a path through the scene using RoadManager.
    /// </summary>
    /// <returns>Array of waypoint positions, or null if none found.</returns>
    public Vector3[] FindRoute()
    {
        List<Vector3> path = RoadManager.Instance.SearchForPath();
        if (path.Count == 0) return null;
        return path.ToArray();
    }

    /// <summary>
    /// Starts the tour for a vehicle if all passengers have boarded.
    /// </summary>
    /// <param name="ID">ID of the vehicle starting its tour.</param>
    public void StartTour(int ID)
    {
        var vehicle = activeVehicles.Find(t => t.GetID() == ID);
        var view = vehicleViews[vehicle.GetID()];
        if (vehicle != null && view != null && vehicle.AllPassengersArrived())
        {
            vehicle.State = VehicleState.On_tour;

            vehicle.SetTouristState(TouristState.On_tour);

            var WayPoints = FindRoute();

            view.MoveOnRoute(WayPoints);

        }
    }

    /// <summary>
    /// Completes the tour, returns vehicle to parking, and clears its passengers.
    /// </summary>
    /// <param name="ID">ID of the vehicle finishing its tour.</param>
    public void FinishTour(int ID)
    {
        var vehicle = activeVehicles.Find(t => t.GetID() == ID);
        var view = vehicleViews[vehicle.GetID()];
        if (vehicle != null && view != null)
        {
            vehicle.SetTouristState(TouristState.Finished);

            vehicle.State = VehicleState.Busy;

            view.MoveTo(AssignVehicleToParkingSpot());

            vehicle.ClearPassengers();

        }
    }

    /// <summary>
    /// Adjusts the maximum vehicle capacity by a specified amount.
    /// </summary>
    /// <param name="amount">Change in capacity (positive or negative).</param>
    public void UpdateCapacity(int amount)
    {
        capacity += amount;
    }

    /// <summary>
    /// Loads all VehicleData ScriptableObjects from the Resources/Vehicles folder.
    /// </summary>
    private void LoadAllVehicles()
    {
        vehicleDatabase = new List<VehicleData>(Resources.LoadAll<VehicleData>("Vehicles"));
    }
}

/// <summary>
/// Enumeration of possible states a vehicle can be in during its lifecycle.
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