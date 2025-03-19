using System;
using System.Collections.Generic;
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

    // Egyéni ID minden járműnek
    private static int nextID = 0;

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
        LoadAllVehicles();
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        //SpawnVehicle(VehicleType.BUS);
        SpawnVehicle(VehicleType.JEEP);
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var vehicle in activeVehicles)
        {
            float delta = Time.deltaTime;

            switch (vehicle.GetState())
            {
                case VehicleState.ON_TOUR:
                    // Ez nem biztos, hogy a leghatékonyabb, lehetséges, hogy a Touristok maguk néznék az autót, hogy éppen hol van
                    vehicle.UpdateTouristPosition(vehicleViews[vehicle.GetID()].transform.position);
                    break;
                case VehicleState.FULL:
                    StartTour(vehicle.GetID());
                    break;
                case VehicleState.FINISHED:
                    FinishTour(vehicle.GetID());
                    break;
                case VehicleState.WAITING:
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
        Vehicle newVehicle = new Vehicle(GenerateID(), selectedData);
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
        var filteredVehicles = activeVehicles.FindAll(t => t.GetState() != VehicleState.ON_TOUR && t.GetState() != VehicleState.FINISHED && t.GetState() != VehicleState.BUSY);

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
        return new Vector3(-7f, 0f, -5f);
    }

    // Keres egy utat amin elindítja az autót
    public Vector3[] FindRoute()
    {
        // TODO
        Vector3[] list = new Vector3[4];
        list[0] = new Vector3(-6, 0, 13);
        list[1] = (new Vector3(4, 0, 13));
        list[2] = (new Vector3(4, 0, -7));
        list[3] = (new Vector3(-6, 0, -7));
        return list;
    }

    // Elindítja a túrát
    public void StartTour(int ID)
    {
        var vehicle = activeVehicles.Find(t => t.GetID() == ID);
        var view = vehicleViews[vehicle.GetID()];
        if (vehicle != null && view != null && vehicle.AllPassengersArrived())
        {
            vehicle.State = VehicleState.ON_TOUR;

            vehicle.SetTouristState(TouristState.ON_TOUR);

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
            vehicle.SetTouristState(TouristState.FINISHED);

            vehicle.State = VehicleState.BUSY;

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

    // Generál egy új ID-t
    public int GenerateID()
    {
        nextID++;
        return nextID;
    }

}
