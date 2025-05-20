using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// Manages all Feeder structures in the game.
/// Responsible for creating, storing, removing, and refilling Feeders.
/// Implements the IStructureManager interface.
/// </summary>
public class FeederManager : MonoBehaviour, IStructureManager
{
    /// <summary>
    /// Singleton instance of the FeederManager.
    /// </summary>
    public static FeederManager Instance;

    /// <summary>
    /// List of currently active Feeders in the game.
    /// </summary>
    private List<FeederModel> ActiveFeeders = new List<FeederModel>();

    /// <summary>
    /// Ensures the Singleton pattern and prevents duplicate instances.
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
    /// Creates a new Feeder structure and stores it in the active list.
    /// </summary>
    /// <param name="data">The BuildingData used to create the Feeder.</param>
    /// <param name="ID">The unique identifier for the structure.</param>
    /// <param name="gridPosition">The position on the grid (not used directly here).</param>
    /// <returns>The created Feeder instance as a Structure.</returns>
    public Structure AddStructure(BuildingData data, int ID, Vector2Int gridPosition)
    {
        FeederModel Feeder = new FeederModel(data, ID);
        if (Feeder == null) throw new Exception("Failed to create Feeder.");

        ActiveFeeders.Add(Feeder);

        return Feeder;
    }


    /// <summary>
    /// Reconstructs a Feeder structure from save data and stores it.
    /// </summary>
    /// <param name="data">The BuildingData for the Feeder.</param>
    /// <param name="saveData">The saved structure data to restore from.</param>
    /// <param name="gridPosition">The grid position of the Feeder (not used directly here).</param>
    /// <returns>The restored Feeder instance as a Structure.</returns>
    public Structure RegisterStructure(BuildingData data, StructureSaveData saveData, Vector2Int gridPosition)
    {
        FeederModel Feeder = new FeederModel(data, saveData);
        if (Feeder == null) throw new Exception("Failed to create Feeder from save data.");

        ActiveFeeders.Add(Feeder);

        return Feeder;
    }


    /// <summary>
    /// Removes a Feeder from the manager by its ID.
    /// </summary>
    /// <param name="ID">The ID of the Feeder to remove.</param>
    public void RemoveStructure(int ID)
    {
        FeederModel Feeder = ActiveFeeders.Find(t => t.GetID() == ID);
        if (Feeder == null) return;

        ActiveFeeders.Remove(Feeder);
    }


    /// <summary>
    /// Refills the Feeder with the given ID, if the player has enough money.
    /// Grants EXP for successful refill.
    /// </summary>
    /// <param name="ID">The ID of the Feeder to refill.</param>
    /// <param name="Price">The cost of the refill operation.</param>
    public void Refill(int ID, int Price)
    {
        if (EconomyManager.Instance.HasEnoughMoney(Price))
            ActiveFeeders.Find(t => t.GetID() == ID).Refill();
        else Debug.LogWarning("Not enough money to refill!");

        GameEvents.Instance.NotifyObservers(EventType.EXP_GAIN, 20);
    }


    /// <summary>
    /// Sets the view reference for a structure (currently unused for Feeders).
    /// </summary>
    /// <param name="view">The view implementing IPlaceable.</param>
    /// <param name="ID">The ID of the structure to link the view with.</param>
    public void SetView(IPlaceable view, int ID) { }
}
