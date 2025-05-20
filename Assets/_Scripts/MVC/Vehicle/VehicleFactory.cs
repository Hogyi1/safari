using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Factory responsible for creating Vehicle instances by loading VehicleData
/// assets and instantiating associated prefabs with their model and view.
/// </summary>
public class VehicleFactory : MonoBehaviour
{
    /// <summary>
    /// In-memory cache of all VehicleData assets loaded from Resources.
    /// Used to avoid repeated loading and improve performance.
    /// </summary>
    private List<VehicleData> vehicleDatabase = new List<VehicleData>();


    /// <summary>
    /// Parent transform under which all vehicle GameObjects are instantiated.
    /// Helps keep the hierarchy organized.
    /// </summary>
    [SerializeField] private GameObject VehicleParent;


    /// <summary>
    /// Unity Start callback. Loads all VehicleData assets into the cache
    /// at the beginning of the game.
    /// </summary>
    private void Start()
    {
        LoadAllVehicles();
    }


    /// <summary>
    /// Creates a new Vehicle by type ID, instantiating its prefab at the
    /// default parking spot, and initializing its view and model components.
    /// </summary>
    /// <param name="id">Unique runtime identifier for the Vehicle instance.</param>
    /// <param name="vehicleDataId">The VehicleData ID used to select prefab and attributes.</param>
    /// <returns>A fully constructed <see cref="Vehicle"/> object, or null if data not found.</returns>
    public Vehicle CreateVehicle(int id, int vehicleDataId)
    {
        var data = FindVehicleData(vehicleDataId);
        if (data == null)
            return null;

        Vector3 position = VehicleManager.Instance.GetParkingSpot();
        var instance = Instantiate(data.VehiclePrefab, position, Quaternion.identity);
        instance.transform.SetParent(VehicleParent.transform, true);

        var view = instance.GetComponent<VehicleView>();
        view.SetSpeed(data.Speed);
        view.gameObject.SetActive(false);

        var model = new VehicleModel(id, data);
        return new Vehicle(id, model, view);
    }


    /// <summary>
    /// Recreates a Vehicle from its saved state data, restoring position, route, and state.
    /// </summary>
    /// <param name="vehicleData">Previously saved vehicle data to restore from.</param>
    /// <returns>A fully reconstructed <see cref="Vehicle"/> instance.</returns>
    public Vehicle CreateVehicle(VehicleSaveData vehicleData)
    {
        var data = FindVehicleData(vehicleData.Type);
        if (data == null)
            return null;

        Vector3 position = vehicleData.CurrentPosition;
        var instance = Instantiate(data.VehiclePrefab, position, Quaternion.identity);
        instance.transform.SetParent(VehicleParent.transform, true);

        var view = instance.GetComponent<VehicleView>();
        view.SetSpeed(data.Speed);

        if (vehicleData.CurrentRoute.Count > 0)
        {
            view.MoveOnRoute(vehicleData.CurrentRoute, vehicleData.NextState);
            // No need to restore AnimalsInView; OnTrigger will repopulate it
        }
        else
        {
            view.gameObject.SetActive(false);
        }

        var model = new VehicleModel(vehicleData, data);
        return new Vehicle(vehicleData.ID, model, view);
    }


    /// <summary>
    /// Finds and returns the VehicleData asset that matches the given VehicleID.
    /// </summary>
    /// <param name="vehicleDataId">The ID of the vehicle type to retrieve.</param>
    /// <returns>Corresponding <see cref="VehicleData"/> object or null if not found.</returns>
    private VehicleData FindVehicleData(int vehicleDataId)
    {
        return vehicleDatabase.Find(v => v.VehicleID == vehicleDataId);
    }


    /// <summary>
    /// Finds and returns the VehicleData asset that matches the given VehicleType enum.
    /// </summary>
    /// <param name="type">The <see cref="VehicleType"/> to search for.</param>
    /// <returns>Matching <see cref="VehicleData"/> or null if not found.</returns>
    private VehicleData FindVehicleData(VehicleType type)
    {
        return vehicleDatabase.Find(v => v.Type == type);
    }


    /// <summary>
    /// Loads all VehicleData ScriptableObjects from the "Resources/Vehicles" folder
    /// into memory at the beginning of runtime.
    /// </summary>
    private void LoadAllVehicles()
    {
        vehicleDatabase = new List<VehicleData>(Resources.LoadAll<VehicleData>("Vehicles"));
        Debug.Log($"Loaded {vehicleDatabase.Count} vehicle configurations.");
    }
}
