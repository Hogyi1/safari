using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Factory responsible for creating Vehicle instances by loading VehicleData
/// assets and instantiating associated prefabs with their model and view.
/// </summary>
public class RangerFactory : MonoBehaviour
{
    /// <summary>
    /// In-memory cache of all VehicleData assets loaded from Resources.
    /// </summary>
    private List<RangerData> rangerDatabase = new List<RangerData>();
    [SerializeField] private GameObject RangerParent;

    /// <summary>
    /// Unity Start callback. Loads all VehicleData assets into the cache.
    /// </summary>
    private void Start()
    {
        LoadAllRangers();
    }

    /// <summary>
    /// Creates a new Vehicle, instantiating its prefab at the given position,
    /// setting up its view and model components, and returning the assembled object.
    /// </summary>
    /// <param name="id">Unique runtime identifier for the Vehicle instance.</param>
    /// <param name="rangerDataID">Identifier used to look up the VehicleData asset.</param>
    /// <param name="position">World position where the vehicle prefab will be spawned.</param>
    /// <returns>
    /// A new <see cref="Ranger"/> object combining ID, model, and view,
    /// or null if no matching VehicleData was found.
    /// </returns>
    public Ranger CreateRanger(int id, int rangerDataID)
    {
        var data = FindRangerData(rangerDataID);
        if (data == null)
            return null;
        Vector3 position = RangerManager.Instance.GetSpawnposition();
        var instance = Instantiate(data.RangerPrefab, position, Quaternion.identity);
        instance.transform.SetParent(RangerParent.transform, true);

        var view = instance.GetComponent<RangerView>();
        view.gameObject.SetActive(false);

        var model = new RangerModel(id, data);
        return new Ranger(id, model, view);
    }

    /// <summary>
    /// Finds the VehicleData asset matching the given ID in the loaded cache.
    /// </summary>
    /// <param name="rangerDataID">Identifier to match against VehicleData.VehicleID.</param>
    /// <returns>
    /// The matching <see cref="RangerData"/> asset, or null if not found.
    /// </returns>
    private RangerData FindRangerData(int rangerDataID)
    {
        return rangerDatabase.Find(v => v.RangerID == rangerDataID);
    }

    /// <summary>
    /// Loads all VehicleData ScriptableObjects from the "Vehicles" Resources folder
    /// into the in-memory cache.
    /// </summary>
    private void LoadAllRangers()
    {
        rangerDatabase = new List<RangerData>(Resources.LoadAll<RangerData>("Rangers"));
        Debug.Log($"Loaded {rangerDatabase.Count} ranger configurations.");
    }
}
