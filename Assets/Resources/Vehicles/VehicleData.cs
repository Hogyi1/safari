using UnityEngine;

/// <summary>
/// Represents data for a single vehicle entity, including stats such as speed, capacity, and prefab reference.
/// Implements the IDetails interface for use in UI display.
/// </summary>
[CreateAssetMenu(fileName = "VehicleData", menuName = "Scriptable Objects/VehicleData")]
public class VehicleData : ScriptableObject, IDetails
{
    /// <summary>
    /// Number of passengers or units the vehicle can transport.
    /// </summary>
    [Range(1, 10)]
    public int Capacity;

    /// <summary>
    /// Unique identifier for this vehicle type.
    /// </summary>
    public int VehicleID;

    /// <summary>
    /// Movement speed of the vehicle.
    /// </summary>
    public float Speed = 5;

    /// <summary>
    /// How many grid units this vehicle occupies on the map.
    /// Used for placement or collision purposes.
    /// </summary>
    [Range(1, 3)]
    public int SpaceTaken;

    /// <summary>
    /// The type/category of the vehicle (e.g., Truck, Jeep, Helicopter).
    /// </summary>
    public VehicleType Type;

    /// <summary>
    /// Prefab reference used to instantiate the vehicle in the game world.
    /// </summary>
    public GameObject VehiclePrefab;

    /// <summary>
    /// The in-game currency cost to acquire this vehicle.
    /// </summary>
    public int Price;

    /// <summary>
    /// Returns the vehicle's unique identifier.
    /// </summary>
    /// <returns>The VehicleID of this vehicle.</returns>
    public int GetDataID()
    {
        return VehicleID;
    }

    /// <summary>
    /// Returns the string representation of the vehicle's type.
    /// </summary>
    /// <returns>Display string for the vehicle's category/type.</returns>
    public string GetDisplayType()
    {
        return Type.ToString();
    }

    /// <summary>
    /// Returns a detail object representing the amount of space the vehicle occupies.
    /// </summary>
    /// <returns>An OtherDetail object with the label "Occupied Space" and value as SpaceTaken.</returns>
    public OtherDetail GetOtherDetail()
    {
        return new OtherDetail("Occupied Space:", SpaceTaken.ToString(), "Capacity", Capacity.ToString());
    }

    /// <summary>
    /// Returns the price of the vehicle in in-game currency.
    /// </summary>
    /// <returns>The purchase price of the vehicle.</returns>
    public int GetPrice()
    {
        return Price;
    }
}
