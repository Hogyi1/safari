using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static VehicleState;

/// <summary>
/// Controls the lifecycle and routing of vehicles, assigns tourists to vehicles,
/// and manages tour start and finish in an MVC architecture.
/// </summary>
[RequireComponent(typeof(VehicleFactory))]
public class VehicleManager : MonoBehaviour, IUpgradeable
{
    /// <summary>
    /// Singleton instance of the VehicleManager.
    /// </summary>
    public static VehicleManager Instance { get; private set; }

    [SerializeField] private VehicleFactory factory;

    [Tooltip("Which manager type is mine")]
    [SerializeField] private ManagerType myType = ManagerType.Vehicle;
    [Tooltip("The position, where the vehicles should return to")]
    [SerializeField] private GameObject garage;
    [Tooltip("Maximum waiting time per vehicle in seconds")]
    [SerializeField] private const float maxWaitingTime = 15f;
    [Tooltip("The amount of an upgrade session")]
    [SerializeField] private const int upgradeAmount = 2;


    private List<Vehicle> activeVehicles = new List<Vehicle>();
    private int capacity = 5;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        RoadManager.Instance.OnRoadRemoved += HandleRedirect;
    }

    private void Update()
    {
        float delta = Time.deltaTime;

        foreach (var vehicle in activeVehicles)
        {
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
        if (capacity <= activeVehicles.Count) return;
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
                return vehicle.ID;
        }

        return -1;
    }

    /// <summary>
    /// Determines the parking spot position for spawning vehicles.
    /// </summary>
    public Vector3 GetParkingSpot()
    {
        Vector3 pos;
        try
        {
            pos = garage.transform.Find("Garage").position;
        }
        catch (Exception ex)
        {
            Debug.LogWarning("Nincsen beállítva Garage az alap beállításokat fogom használni");
            pos = garage.GetComponent<Renderer>().bounds.center;
        }
        return pos;
    }

    /// <summary>
    /// Finds a random route via the RoadManager.
    /// </summary>
    private List<Vector3> FindRoute()
    {
        return RoadManager.Instance.SearchForRandomPath();
    }

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

    public void SetVehicleState(int iD, VehicleState newState)
    {
        activeVehicles.Find(t => t.ID == iD).Model.State = newState;
    }

    private void HandleRedirect()
    {
        Debug.Log("Redirecting rn");
        var vehiclesOnTour = activeVehicles.FindAll(t => t.Model.State == On_tour || t.Model.State == Busy);

        foreach (var vehicle in vehiclesOnTour)
        {
            bool toExit = vehicle.Model.State == On_tour;
            Debug.Log("ToExit? " + toExit);
            List<Vector3> newPath = RoadManager.Instance.FindNewPath(vehicle.View.transform.position, toExit);
            vehicle.View.StopAllCoroutines();
            Debug.Log("current state " + vehicle.Model.State);
            Debug.Log("do i have a path " + (newPath.Count != 0));

            if (newPath.Count == 0 && toExit) vehicle.Model.State = Finished;
            else if (newPath.Count == 0 && !toExit) vehicle.View.ResetVehicle();
            else if (toExit) vehicle.View.MoveOnRoute(newPath, Finished);
            else vehicle.View.MoveOnRoute(newPath, Empty);
        }
    }

<<<<<<< HEAD
    public List<AnimalType> GetAnimalsInSight(int vehicleID)
=======
    /// <summary>
    /// Assigns a tourist to the first available vehicle and returns its door position.
    /// </summary>
    /// <param name="tourist">Tourist to assign.</param>
    /// <returns>Door position for pickup, or null if none available.</returns>
    public Vector3? AssignTouristToVehicle(Tourist tourist)
>>>>>>> 923712d8f96365157209ba049298a2f508440c16
    {
        return activeVehicles.Find(t => t.ID == vehicleID).View.animalsInView;
    }

<<<<<<< HEAD
    public Vector3 GetGaragePosition(int ID)
    {
        return FacilityManager.Instance.GetInteractingPosition(myType);
    }

    public void LevelUp()
=======
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
>>>>>>> 923712d8f96365157209ba049298a2f508440c16
    {
        capacity += upgradeAmount;
    }

<<<<<<< HEAD
    public void LevelDown()
    {
        capacity -= upgradeAmount;
    }

    public int MaxCapacity => capacity;
    public int Capacity => activeVehicles.Count;
}

/// <summary>
/// Enumerates possible states of a vehicle's lifecycle.
=======
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
>>>>>>> 923712d8f96365157209ba049298a2f508440c16
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