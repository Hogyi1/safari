using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// Manages all vegetation-type structures in the game, including their models and views.
/// Handles registration, removal, and regrowth behavior via random events.
/// Implements the IStructureManager and IRandomEventObserver interfaces.
/// </summary>
public class VegetationManager : MonoBehaviour, IStructureManager, IRandomEventObserver
{
    /// <summary>
    /// Singleton instance of the VegetationManager.
    /// </summary>
    public static VegetationManager Instance;

    /// <summary>
    /// List of all active vegetation models currently tracked.
    /// </summary>
    private List<VegetationModel> activeVegetations = new List<VegetationModel>();

    /// <summary>
    /// Dictionary mapping structure IDs to their corresponding VegetationView instances.
    /// </summary>
    private Dictionary<int, VegetationView> activeViews = new Dictionary<int, VegetationView>();


    /// <summary>
    /// Initializes the singleton instance and ensures it persists across scene loads.
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
    /// Subscribes this manager to random event notifications.
    /// </summary>
    void Start()
    {
        RandomEvents.Instance.AddObserver(this);
    }


    /// <summary>
    /// Creates a new vegetation model instance and registers it internally.
    /// </summary>
    /// <param name="data">Building data describing the vegetation type.</param>
    /// <param name="ID">The unique identifier for this structure.</param>
    /// <param name="gridPosition">The position on the map grid (unused).</param>
    /// <returns>The created Structure (Vegetation) model instance.</returns>
    public Structure AddStructure(BuildingData data, int ID, Vector2Int gridPosition)
    {
        VegetationModel Vegetation = new VegetationModel(data, ID);
        if (Vegetation == null) throw new Exception("Nem sikerült léterhozni a következőt: Vegetation");

        activeVegetations.Add(Vegetation);

        return Vegetation;
    }


    /// <summary>
    /// Reconstructs and registers a vegetation model from saved data.
    /// </summary>
    /// <param name="data">Building data describing the vegetation.</param>
    /// <param name="saveData">The saved state of the vegetation.</param>
    /// <param name="gridPosition">The position on the map grid (unused).</param>
    /// <returns>The restored Structure (Vegetation) model.</returns>
    public Structure RegisterStructure(BuildingData data, StructureSaveData saveData, Vector2Int gridPosition)
    {
        VegetationModel Vegetation = new VegetationModel(data, saveData);
        if (Vegetation == null) throw new Exception("Nem sikerült léterhozni a következőt: Vegetation");

        activeVegetations.Add(Vegetation);

        return Vegetation;
    }


    /// <summary>
    /// Removes a vegetation structure by its unique ID.
    /// </summary>
    /// <param name="ID">The ID of the structure to remove.</param>
    public void RemoveStructure(int ID)
    {
        VegetationModel Vegetation = activeVegetations.Find(t => t.GetID() == ID);
        if (Vegetation == null) return;

        activeVegetations.Remove(Vegetation);
        activeViews.Remove(ID);
    }


    /// <summary>
    /// Randomly regrows capacity for each vegetation instance based on a fraction of its max capacity.
    /// </summary>
    private void HandleRegrowEvent()
    {
        foreach (VegetationModel Vegetation in activeVegetations)
        {
            int RandomAmount = UnityEngine.Random.Range(0, Vegetation.GetMaxCapacity() / 3);
            Vegetation.Regrow(RandomAmount);
        }
    }


    /// <summary>
    /// Handles random event notifications. If "Regrow" event, triggers regrowth on all vegetation.
    /// </summary>
    /// <param name="randomEvent">The received random event type.</param>
    public void OnNotify(RandomEvent randomEvent)
    {
        if (randomEvent == RandomEvent.Regrow)
            HandleRegrowEvent();
    }


    /// <summary>
    /// Binds a view (IPlaceable) to a vegetation model by its ID.
    /// </summary>
    /// <param name="view">The view instance to bind.</param>
    /// <param name="ID">The structure's unique ID.</param>
    public void SetView(IPlaceable view, int ID)
    {
        activeViews[ID] = (VegetationView)view;
    }
}
