using UnityEngine;

/// <summary>
/// Serializable data structure used for saving and restoring placed structures in the world.
/// Contains basic information such as unique ID, template reference, position, and rotation.
/// </summary>
[System.Serializable]
public class MapSaveData
{
    /// <summary>
    /// The unique instance ID of the placed structure.
    /// </summary>
    public int UniqueID;

    /// <summary>
    /// The ID referencing the original BuildingData template.
    /// </summary>
    public int BuildingID;

    /// <summary>
    /// The world position where the structure is placed.
    /// </summary>
    public Vector3 Position;

    /// <summary>
    /// The rotation of the structure in world space.
    /// </summary>
    public Quaternion Rotation;

    /// <summary>
    /// Constructs a new MapSaveData entry with specified position and rotation.
    /// </summary>
    /// <param name="uniqueID">The unique ID of the placed structure instance.</param>
    /// <param name="buildID">The ID of the building template (BuildingData).</param>
    /// <param name="position">The world-space position of the structure.</param>
    /// <param name="rotation">The world-space rotation of the structure.</param>
    public MapSaveData(int uniqueID, int buildID, Vector3 position, Quaternion rotation)
    {
        UniqueID = uniqueID;
        BuildingID = buildID;
        Position = position;
        Rotation = rotation;
    }
}
