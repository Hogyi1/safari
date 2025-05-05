using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class VehicleManager : MonoBehaviour
{
    // Az összes ScriptableObject amit használunk
    [SerializeField]
    public static List<VehicleData> vehicleDatabase = new List<VehicleData>();

    // A lerakott autók listája
    private List<Vehicle> activeVehicles = new List<Vehicle>();
    private Dictionary<int, VehicleView> vehicleViews = new Dictionary<int, VehicleView>();

    // Mennyit várhat egy jármű Real time másodpercben
    [SerializeField]
    private readonly float MAX_WAITING_TIME = 15f;

    // Ennyi autó lehet egyszerre
    public int capacity = 5;
    // Singleton pattern
    public static VehicleManager Instance { get; private set; }
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

    void Start()
    {
        LoadAllVehicles();
        SpawnVehicle(VehicleType.Jeep);
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var vehicle in activeVehicles)
        {
            float delta = Time.deltaTime;

            switch (vehicle.GetState())
            {
                case VehicleState.On_tour:
                    // Ez nem biztos, hogy a leghatékonyabb, lehetséges, hogy a Touristok maguk néznék az autót, hogy éppen hol van
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
    // Létrehoz egy új autót
    public void SpawnVehicle(VehicleType Type)
    {
        var selectedData = vehicleDatabase.Find(t => t.type == Type);
        Vehicle newVehicle = new Vehicle(IDGenerator.GenerateID(), selectedData);
        activeVehicles.Add(newVehicle);
        GameObject newVehicleGO = Instantiate(selectedData.vehiclePrefab, AssignVehicleToParkingSpot(), Quaternion.identity);
        VehicleView view = newVehicleGO.GetComponent<VehicleView>();
        view.Init(newVehicle);
        vehicleViews[newVehicle.GetID()] = view;
        Debug.Log($"Új jármű ID: {newVehicle.GetID()}");

    }

    // Eltávolítja az autót
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

    /* 
     * Megpróbálja hozzáadni a túristát az egyik még varakozó autóhoz, 
     * ha sikeres akkor visszaadja az autó helyzetét, ha sikertelen akkor null-t
     * @param Tourist tourist - A hozzáadandó túrista
     */
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

    // Megadja hova térjen vissza az autó miután végzett
    public Vector3 AssignVehicleToParkingSpot()
    {
        // TODO
        // Majd itt meg kell valósítani a Map / ParkingManagert
        return new Vector3(39f, 1.05f, 8.0f);
    }

    // Keres egy utat amin elindítja az autót
    public Vector3[] FindRoute()
    {
        List<Vector3> path = RoadManager.Instance.SearchForPath();
        if (path.Count == 0) return null;
        return path.ToArray();
    }

    // Elindítja a túrát
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

        Debug.Log("Túra elindítva a következő járműnek: " + ID);
    }

    // Befejezi a túrát
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

        Debug.Log("Túra befejezve a következő járműnek: " + ID);

    }

    // Ha fejlesztem / építek egy új parkolót / eladom akkor -1
    public void UpdateCapacity(int amount)
    {
        capacity += amount;
    }

    // Betölti a Resource folderból az összes VehicleData ScriptableObjectet
    private void LoadAllVehicles()
    {
        vehicleDatabase = new List<VehicleData>(Resources.LoadAll<VehicleData>("Vehicles"));
        Debug.Log($"Betöltve {vehicleDatabase.Count} jármű.");
    }
}

public enum VehicleState
{
    Empty,
    Waiting,
    Full,
    On_tour,
    Finished,
    Busy
}