using System.Collections;
using UnityEngine;

/// <summary>
/// Manages the current park's basic information and persists data between sessions.
/// Uses singleton pattern and implements data persistence for save/load functionality.
/// </summary>
public class Park : MonoBehaviour, IDataPersistence
{
    /// <summary>
    /// The unique identifier of the park (not currently used in persistence).
    /// </summary>
    public int ID;

    /// <summary>
    /// Singleton instance of the Park class.
    /// </summary>
    public static Park Instance;

    /// <summary>
    /// The name of the park, editable and saved between sessions.
    /// </summary>
    public string ParkName;

    /// <summary>
    /// The difficulty level of the current park session.
    /// </summary>
    public DifficultyEnum Difficulty;

    public float Priority => 5000f;

    /// <summary>
    /// Ensures only one Park instance exists and persists across scenes.
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
    /// Loads park-specific data from the provided GameData object.
    /// </summary>
    /// <param name="data">The GameData object containing saved park information.</param>
    public IEnumerator LoadData(GameData data)
    {
        ParkName = data.parkData.ParkName;
        Difficulty = data.parkData.Difficulty;

        yield return null;
    }

    /// <summary>
    /// Saves the current park's data into the provided GameData object.
    /// </summary>
    /// <param name="data">The GameData object to write park data into.</param>
    public void SaveData(GameData data)
    {
        data.parkData.ParkName = ParkName;
        data.parkData.Difficulty = Difficulty;
    }

}
