using UnityEngine;

/// <summary>
/// Defines the contract for saving and loading persistent game data.
/// Any MonoBehaviour implementing this interface will be included in the
/// game's save/load system and receive relevant data.
/// </summary>
public interface IDataPersistence
{
    /// <summary>
    /// Loads data from the given GameData object into this component.
    /// </summary>
    /// <param name="data">The GameData object to load values from.</param>
    void LoadData(GameData data);

    /// <summary>
    /// Saves this component's state into the given GameData object.
    /// </summary>
    /// <param name="data">The GameData object to store values in.</param>
    void SaveData(GameData data);

    //void Register(T type);
}