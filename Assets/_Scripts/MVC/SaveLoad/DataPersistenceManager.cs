using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataPersistenceManager : MonoBehaviour
{

    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    [SerializeField] private bool useEncryption;

    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;

    private string selectedProfileId = "";

    public static DataPersistenceManager Instance { get; private set; }


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
        this.selectedProfileId = dataHandler.GetMostRecentlyUpdatedProfileId();
        dataPersistenceObjects = FindAllDataPersistenceObjects();
        SceneManager.sceneLoaded += OnSceneLoaded;
        this.LoadGame();

    }


    /// <summary>
    /// Starts a New Game
    /// </summary>
    public void NewGame()
    {
        this.gameData = new GameData();
        CreateParkManager.Instance.SetParkPropertys();
        gameData.parkData.parkName = Park.Instance.ParkName;
        SaveGame();
    }


    /// <summary>
    /// Loads an existing game using the selected profile ID.
    /// Distributes the loaded data to all objects implementing IDataPersistence.
    /// Then saves the game state to ensure consistency.
    /// </summary>
    public void LoadGame()
    {
        dataPersistenceObjects = FindAllDataPersistenceObjects();
        this.gameData = dataHandler.Load(selectedProfileId);

        IDGenerator.SetSeed(gameData.idSeed);

        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }
        // SaveGame();
    }


    // <summary>
    /// Saves the current game state for the selected profile ID.
    /// Collects data from all objects implementing IDataPersistence,
    /// updates the last modified timestamp, and writes the data to file.
    /// </summary>
    public void SaveGame()
    {
        dataPersistenceObjects = FindAllDataPersistenceObjects();
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(gameData);
        }

        //timeStemp
        gameData.lastUpdated = System.DateTime.Now.ToBinary();

        //Filebaírás
        dataHandler.Save(gameData, selectedProfileId);
    }

    /// <summary>
    /// Finds all active and inactive MonoBehaviour components in the scene
    /// that implement the IDataPersistence interface.
    /// </summary>
    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .OfType<IDataPersistence>();
        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    /// <summary>
    /// Changes the currently selected profile ID for loading and saving game data.
    /// This does not load or save immediately, but updates the internal reference.
    /// </summary>
    public void ChangeSelectedProfileId(string newProfileId)
    {
        // update the profile to use for saving and loading
        this.selectedProfileId = newProfileId;
        // load the game, which will use that profile, updating our game data accordingly
    }

    /// <summary>
    /// Checks whether valid game data is currently loaded in memory.
    /// </summary>
    /// <returns>True if game data exists, false otherwise.</returns>
    ///</summary>
    public bool HasGameData()
    {
        return gameData != null;
    }

    /// <summary>
    /// Retrieves all saved game data for all available player profiles.
    /// </summary>
    /// <returns>A dictionary mapping profile IDs to their corresponding game data.</returns>
    /// </summary>
    public Dictionary<string, GameData> GetAllProfilesGameData()
    {
        return dataHandler.LoadAllProfiles();
    }



    /// <summary>
    /// Callback invoked when a new scene is loaded. 
    /// Re-discovers all data persistence objects in the scene and loads the game data for them.
    /// </summary>
    /// <param name="scene">The scene that was loaded.</param>
    /// <param name="mode">The load mode used for the scene.</param>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene loaded: " + scene.name);
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }

    /// <summary>
    /// Callback invoked when the application is quitting. 
    /// Automatically saves the current game state before exit.
    /// </summary>
    private void OnApplicationQuit()
    {
        SaveGame();
    }
}
