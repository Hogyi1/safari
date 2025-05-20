using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages all water-related structures in the game,
/// including creation, registration, and removal of water models.
/// Implements the IStructureManager interface.
/// </summary>
public class WaterManager : MonoBehaviour, IStructureManager
{
    /// <summary>
    /// Singleton instance of the WaterManager.
    /// </summary>
    public static WaterManager Instance;


    /// <summary>
    /// List of all active water structures currently managed.
    /// </summary>
    private List<WaterModel> ActiveWaters = new List<WaterModel>();


    /// <summary>
    /// Initializes the singleton instance and ensures it persists between scenes.
    /// </summary>
    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }


    /// <summary>
    /// Creates a new water structure from runtime placement and registers it internally.
    /// </summary>
    /// <param name="data">The building data associated with the structure.</param>
    /// <param name="ID">The unique identifier for the structure.</param>
    /// <param name="gridPosition">The grid position where the structure is placed.</param>
    /// <returns>The created Structure model instance.</returns>
    public Structure AddStructure(BuildingData data, int ID, Vector2Int gridPosition)
    {
        WaterModel water = new WaterModel(data, ID);
        if (water == null) throw new Exception("Nem sikerült léterhozni a következőt: Water");

        ActiveWaters.Add(water);

        Debug.Log("Water placed");
        return water;
    }


    /// <summary>
    /// Registers an existing saved water structure from save data into the manager.
    /// </summary>
    /// <param name="data">The building data associated with the structure.</param>
    /// <param name="saveData">The saved state of the structure.</param>
    /// <param name="gridPosition">The grid position where the structure was originally placed.</param>
    /// <returns>The registered Structure model instance.</returns>
    public Structure RegisterStructure(BuildingData data, StructureSaveData saveData, Vector2Int gridPosition)
    {
        WaterModel water = new WaterModel(data, saveData);
        if (water == null) throw new Exception("Nem sikerült léterhozni a következőt: Water");

        ActiveWaters.Add(water);

        Debug.Log("Water placed");
        return water;
    }


    /// <summary>
    /// Removes the water structure with the specified ID from the manager.
    /// </summary>
    /// <param name="ID">The unique identifier of the structure to remove.</param>
    public void RemoveStructure(int ID)
    {
        WaterModel water = ActiveWaters.Find(t => t.GetID() == ID);
        if (water == null) return;

        ActiveWaters.Remove(water);
    }


    /// <summary>
    /// Binds a visual view to the water model.
    /// Currently unused in this manager.
    /// </summary>
    /// <param name="view">The visual component representing the structure.</param>
    /// <param name="ID">The unique identifier of the structure model.</param>
    public void SetView(IPlaceable view, int ID) { }
}
