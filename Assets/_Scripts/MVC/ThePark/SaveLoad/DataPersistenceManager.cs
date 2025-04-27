using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class DataPersistanceManager : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private bool initializeDataIfNull;

    [Header("File Storige Config")]
    [SerializeField] private string fileName;
    [SerializeField] private bool useEncryption;

    public static DataPersistanceManager Instance;
    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;
    private string selectedProfileId = "";

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
    }


    public void OnSceaneLoaded(Scene scene, LoadSceneMode mode)
    {

        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();

    }

    public void OnSceaneUnloaded(Scene scene)
    {

        SaveGame();
    }

    public void OnApplicationQuit()
    {
        SaveGame();
    }

    public void NewGane()
    {
        this.gameData = new GameData();
    }

    public void LoadGame()
    {

        this.gameData = dataHandler.Load(selectedProfileId);
        if (initializeDataIfNull && this.gameData == null)
        {
            NewGane();
        }

        if (this.gameData == null) return;
        foreach (IDataPersistence persistence in dataPersistenceObjects)
        {

            persistence.LoadData(gameData);

        }
    }
    public void SaveGame()
    {
        if (this.gameData == null) return;


        foreach (IDataPersistence persistence in dataPersistenceObjects)
        {
            persistence.SaveData(gameData);

        }

        dataHandler.Save(gameData, selectedProfileId);
    }


    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {

        IEnumerable<IDataPersistence> dataPersistenceObjects = Resources.FindObjectsOfTypeAll<MonoBehaviour>()
                    .OfType<IDataPersistence>()
                    .ToList();

        return new List<IDataPersistence>(dataPersistenceObjects);

    }

    public bool HasGameData()
    {
        return this.gameData != null;
    }

    public Dictionary<string, GameData> GetAllProfilesGameData()
    {

        return dataHandler.LoadAllProfiles();
    }
    public void ChangeSelectedProdileId(string id) { 
    
        this.selectedProfileId = id;
    
    }
}
