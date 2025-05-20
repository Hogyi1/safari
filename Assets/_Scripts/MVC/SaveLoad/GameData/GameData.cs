using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Represents the full state of the game to be saved or loaded.
/// Contains high-level systems such as park and inventory data.
/// </summary>
[System.Serializable]
public class GameData
{
    /// <summary>
    /// Timestamp of the last save, stored as binary (used for tracking most recent profile).
    /// </summary>
    public long lastUpdated;

    /// <summary>
    /// Example serializable dictionary (int-to-int) for custom data testing or tracking.
    /// </summary>
    public SerializableDictionary<int, int> exemple;

    /// <summary>
    /// Data related to the park state.
    /// </summary>
    public ParkData parkData;

    /// <summary>
    /// Data related to the player's inventory (e.g., items, currencies).
    /// </summary>
    public InventoryData inventoryData;
    public LevelSaveData levelSaveData;
    public GameSettingsModel settingsModel;
    public Economy Economy;
    public int idSeed;
    public Vector3 CameraPosition;
    public Vector3 CameraRotation;
    public TouristData touristData;
    public VehicleSaveData vehicleData;
    public int RangerCount;
    public int RangerMaxCapacity;
    public List<AnimalSaveData> animalSaveDatas;
    public List<SaveMapData> saveMapDatas;
    public List<ChallangeData> challangeDataList;

    /// <summary>
    /// Initializes a new GameData instance with default values.
    /// Called when no previous save exists for a profile.
    /// </summary>
    public GameData()
    {
        this.parkData = new ParkData();
        this.inventoryData = new InventoryData();
        this.levelSaveData = new LevelSaveData();
        this.settingsModel = new GameSettingsModel();
        this.CameraPosition = new Vector3(0, 35, 0);
        this.CameraPosition = new Vector3(68.199f, 0, 0);
        this.Economy = new Economy();
        this.touristData = new TouristData();
        this.vehicleData = new VehicleSaveData();
        this.RangerCount = 0;
        this.RangerMaxCapacity = 3;
        this.saveMapDatas = new();  
        this.challangeDataList = new List<ChallangeData>();
    }
}
