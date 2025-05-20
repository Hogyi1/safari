using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataPersistenceManager : MonoBehaviour
{

    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    [SerializeField] private bool useEncryption;
    [SerializeField] private string newGameProfileID = "0";

    private GameData gameData;
    private FileDataHandler dataHandler;

    private string selectedProfileId = "";

    public static DataPersistenceManager Instance { get; private set; }
    public event Action OnAllLoaded;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
        selectedProfileId = dataHandler.GetMostRecentlyUpdatedProfileId();
        SceneManager.sceneLoaded += OnSceneLoaded;
        StartCoroutine(LoadGame());
    }


    /// <summary>
    /// Starts a New Game
    /// </summary>
    public void NewGame()
    {
        gameData = new GameData();
        dataHandler.LoadAllProfiles().TryGetValue(newGameProfileID, out this.gameData); // Még a mentés előtt betölti az új játékos profilt
        Debug.Log(gameData.IDSeed);
        SaveGame(); // Egyből el is menti a már megváltoztatott profile ID helyére
    }


    /// <summary>
    /// Loads an existing game using the selected profile ID.
    /// Distributes the loaded data to all objects implementing IDataPersistence.
    /// Then saves the game state to ensure consistency.
    /// </summary>
    public IEnumerator LoadGame()
    {
        // Select every IDataPersistance object in order by their Priority (The higher the value the more important it is)
        var orderedDataPersistanceObjects = FindAllDataPersistenceObjects().OrderBy(t => t.Priority).Reverse().ToList();
        gameData = dataHandler.Load(selectedProfileId);

        // Set the most important values by hand to minimize failure of Async loading
        GameSettingsController.Instance.LoadData(gameData);
        IDGenerator.SetSeed(gameData.IDSeed);

        foreach (IDataPersistence dataPersistenceObj in orderedDataPersistanceObjects)
        {
            // Wait until the current manager has loaded all data
            yield return dataPersistenceObj.LoadData(gameData);
        }

        // Invoke when everything has loaded
        OnAllLoaded?.Invoke();
    }


    /// <summary>
    /// Saves the current game state for the selected profile ID.
    /// Collects data from all objects implementing IDataPersistence,
    /// updates the last modified timestamp, and writes the data to file.
    /// </summary>
    public void SaveGame()
    {
        var dataPersistenceObjects = FindAllDataPersistenceObjects();

        gameData.IDSeed = IDGenerator.GetSeed();
        gameData.lastUpdated = DateTime.Now.ToBinary();

        dataPersistenceObjects.ForEach(t => t.SaveData(gameData));

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
    public void ChangeSelectedProfileId(string newProfileId) => selectedProfileId = newProfileId;


    /// <summary>
    /// Checks whether valid game data is currently loaded in memory.
    /// </summary>
    /// <returns>True if game data exists, false otherwise.</returns>
    ///</summary>
    public bool HasGameData() => gameData != null;


    /// <summary>
    /// Retrieves all saved game data for all available player profiles.
    /// </summary>
    /// <returns>A dictionary mapping profile IDs to their corresponding game data.</returns>
    /// </summary>
    public Dictionary<string, GameData> GetAllProfilesGameData() => dataHandler.LoadAllProfiles();


    /// <summary>
    /// Callback invoked when a new scene is loaded. 
    /// Re-discovers all data persistence objects in the scene and loads the game data for them.
    /// </summary>
    /// <param name="scene">The scene that was loaded.</param>
    /// <param name="mode">The load mode used for the scene.</param>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene loaded: " + scene.name);
        StartCoroutine(LoadGame());
    }


    /// <summary>
    /// Callback invoked when the application is quitting. 
    /// Automatically saves the current game state before exit.
    /// </summary>
    private void OnApplicationQuit()
    {
        // TODO Add popup
        SaveGame();
    }
}

public interface ISaveable<T>
{
    T GetSaveData();
}