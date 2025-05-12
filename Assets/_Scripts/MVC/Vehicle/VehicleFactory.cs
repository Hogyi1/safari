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
    /// </summary>
    private List<VehicleData> vehicleDatabase = new List<VehicleData>();
    [SerializeField] private GameObject VehicleParent;

    /// <summary>
    /// Unity Start callback. Loads all VehicleData assets into the cache.
    /// </summary>
    private void Start()
    {
        LoadAllVehicles();
    }

    /// <summary>
    /// Creates a new Vehicle, instantiating its prefab at the given position,
    /// setting up its view and model components, and returning the assembled object.
    /// </summary>
    /// <param name="id">Unique runtime identifier for the Vehicle instance.</param>
    /// <param name="vehicleDataId">Identifier used to look up the VehicleData asset.</param>
    /// <param name="position">World position where the vehicle prefab will be spawned.</param>
    /// <returns>
    /// A new <see cref="Vehicle"/> object combining ID, model, and view,
    /// or null if no matching VehicleData was found.
    /// </returns>
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
    /// Finds the VehicleData asset matching the given ID in the loaded cache.
    /// </summary>
    /// <param name="vehicleDataId">Identifier to match against VehicleData.VehicleID.</param>
    /// <returns>
    /// The matching <see cref="VehicleData"/> asset, or null if not found.
    /// </returns>
    private VehicleData FindVehicleData(int vehicleDataId)
    {
        return vehicleDatabase.Find(v => v.VehicleID == vehicleDataId);
    }

    /// <summary>
    /// Loads all VehicleData ScriptableObjects from the "Vehicles" Resources folder
    /// into the in-memory cache.
    /// </summary>
    private void LoadAllVehicles()
    {
        vehicleDatabase = new List<VehicleData>(Resources.LoadAll<VehicleData>("Vehicles"));
        Debug.Log($"Loaded {vehicleDatabase.Count} vehicle configurations.");
    }
}
