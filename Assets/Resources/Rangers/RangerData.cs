using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents data for a single vehicle entity, including stats such as speed, capacity, and prefab reference.
/// Implements the IDetails interface for use in UI display.
/// </summary>
[CreateAssetMenu(fileName = "RangerData", menuName = "Scriptable Objects/RangerData")]
public class RangerData : ScriptableObject, IDetails
{
    /// <summary>
    /// Unique identifier for this rager.
    /// </summary>
    public int RangerID;

    /// <summary>
    /// The in-game currency cost to acquire this vehicle.
    /// </summary>
    public int Price;

    /// <summary>
    /// Ranger unique name
    /// </summary>
    public string MyName;

    /// <summary>
    /// Predefined ranger names to display
    /// </summary>
    public List<string> RangerNames;

    /// <summary>
    /// Prefab reference used to instantiate the ranger in the game world.
    /// </summary>
    public GameObject RangerPrefab;

    /// <summary>
    /// Returns the vehicle's unique identifier.
    /// </summary>
    /// <returns>The RangerID of this vehicle.</returns>
    public int GetDataID()
    {
        return RangerID;
    }

    /// <summary>
    /// Returns the string representation of the vehicle's type.
    /// </summary>
    /// <returns>Display string for the vehicle's category/type.</returns>
    public string GetDisplayType()
    {
        return "Ranger";
    }

    /// <summary>
    /// Returns a detail object representing the amount of space the vehicle occupies.
    /// </summary>
    /// <returns>An OtherDetail object with the label "Occupied Space" and value as SpaceTaken.</returns>
    public OtherDetail GetOtherDetail()
    {
        return new OtherDetail("Occupied Space:", "1", "Name", GetRandomName());
    }

    /// <summary>
    /// Gets a random name from the list and sets it my name if its null
    /// </summary>
    /// <returns>My name</returns>
    private string GetRandomName()
    {
        if (!string.IsNullOrEmpty(MyName))
            return MyName;

        int index = UnityEngine.Random.Range(0, RangerNames.Count);
        MyName = RangerNames[index];
        return RangerNames[index];
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
