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
    /// Data related to the player's inventory (e.g., items, currencies).
    /// </summary>
    public InventoryData inventoryData;

    public LevelSaveData levelSaveData;



    // Park Data
    public ParkData parkData;

    // ID generator
    public int IDSeed;

    /* REORGANIZE */
    // Unique Tourist Data
    public List<TouristSaveData> touristDatas;
    // TouristManager Data
    public TouristManagerSaveData touristManagerSaveData;

    // Unique Vehicle Data
    public List<VehicleSaveData> vehicleDatas;

    // Unique Map Data
    public List<MapSaveData> mapDatas;
    // Unique Structure Data
    public List<StructureSaveData> structureDatas;

    // Economy
    public Economy Economy;
    // GameTime
    public GameTime GameTime;

    // RangerManager Data
    public RangerManagerSaveData rangerManagerSaveData;

    // Settings
    public GameSettingsModel settingsSaveData;

    // Camera Data
    public CameraSaveData cameraSaveData;

    // Challenges
    public List<ChallengeSaveData> challengeDatas;


    public List<AnimalSaveData> animalSaveDatas;


    /// <summary>
    /// Initializes a new GameData instance with default values.
    /// Called when no previous save exists for a profile.
    /// </summary>
    public GameData()
    {
        // ParkData
        parkData = new();

        // Structures
        mapDatas = new();
        structureDatas = new();

        // Tourists
        touristDatas = new();
        touristManagerSaveData = new TouristManagerSaveData(50, 50, false, 0, 0.001f);

        // Vehicles
        vehicleDatas = new();

        // Rangers
        rangerManagerSaveData = new();

        // Settings
        settingsSaveData = new();

        // Camera
        cameraSaveData = new CameraSaveData(new Vector3(68.199f, 0, 0), new Vector3(0, 35, 0));

        // Economy
        Economy = new();

        // GameTime
        GameTime = new();

        // Challenge
        challengeDatas = new();

        this.inventoryData = new InventoryData();
        this.levelSaveData = new LevelSaveData();


        animalSaveDatas = new();
    }
    /// <summary>
    /// Returns the overall completion percentage of the game.
    /// Currently returns a fixed value of 0%.
    /// </summary>
    /// <returns>An integer representing the completion percentage (0-100).</returns>
    public int GetPercentageComplete()
    {
        return 0;
    }
}

[System.Serializable]
public struct TouristManagerSaveData
{
    public float OverallMood;
    public float OverallWaitingMood;
    public bool Incoming;
    public int TouristCount;
    public float MoodSensitivity;

    public TouristManagerSaveData(float overallMood, float overallWaitingMood, bool incoming, int touristCount, float moodSensitivity)
    {
        OverallMood = overallMood;
        OverallWaitingMood = overallWaitingMood;
        Incoming = incoming;
        TouristCount = touristCount;
        MoodSensitivity = moodSensitivity;
    }
}

[System.Serializable]
public struct VehicleManagerSaveData
{
    public int MaxCapacity;
}

[System.Serializable]
public struct RangerManagerSaveData
{
    public int RangerCount;
}

[System.Serializable]
public struct CameraSaveData
{
    public Vector3 CameraPosition;
    public Vector3 CameraRotation;

    public CameraSaveData(Vector3 cameraPosition, Vector3 cameraRotation)
    {
        CameraPosition = cameraPosition;
        CameraRotation = cameraRotation;
    }
}