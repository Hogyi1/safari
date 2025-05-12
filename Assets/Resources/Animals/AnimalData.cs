using UnityEngine;

/// <summary>
/// Stores all relevant data about an animal, including stats, pricing, and visual representation.
/// Used for in-game spawning, UI display, and simulation logic.
/// </summary>
[CreateAssetMenu(fileName = "AnimalData", menuName = "Scriptable Objects/AnimalData")]
public class AnimalData : ScriptableObject, IDetails
{
    /// <summary>
    /// The maximum age this animal can reach before dying.
    /// </summary>
    public int MaxAge;

    /// <summary>
    /// The dietary classification of the animal (e.g., Herbivore, Carnivore).
    /// </summary>
    public DietType Diet;

    /// <summary>
    /// The specific species or type of this animal.
    /// </summary>
    public AnimalType Type;

    /// <summary>
    /// Movement speed of the animal within the simulation.
    /// </summary>
    public float Speed;

    /// <summary>
    /// Unique identifier assigned to this animal.
    /// </summary>
    public int AnimalID;

    /// <summary>
    /// Cost of acquiring or spawning the animal.
    /// </summary>
    public int Price;

    /// <summary>
    /// The prefab used to visually instantiate the animal in the world.
    /// </summary>
    public GameObject AnimalPrefab;

    /// <summary>
    /// Returns the unique data ID for this animal (used in IDetails).
    /// </summary>
    public int GetDataID()
    {
        return AnimalID;
    }

    /// <summary>
    /// Returns a string representing the animal's diet type.
    /// </summary>
    public string GetDisplayType()
    {
        return Diet.ToString();
    }

    /// <summary>
    /// Returns a displayable detail describing the animal's lifespan.
    /// </summary>
    public OtherDetail GetOtherDetail()
    {
        return new OtherDetail("Expected lifespan", MaxAge.ToString(), "Speed", Speed.ToString());
    }

    /// <summary>
    /// Returns the price of the animal in in-game currency.
    /// </summary>
    public int GetPrice()
    {
        return Price;
    }
}
