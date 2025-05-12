using UnityEngine;

/// <summary>
/// Contains all data relevant to a building, including placement info, pricing, capacity,
/// visual representation, and upgrade logic. Used both in runtime and editor for configuration.
/// </summary>
[CreateAssetMenu(fileName = "BuildingData", menuName = "Scriptable Objects/BuildingData")]
public class BuildingData : ScriptableObject, IDetails
{
    /// <summary>
    /// Unique identifier for this building. Used for logic, not to be confused with view ID.
    /// </summary>
    [Tooltip("EZ EGY EGYEDI AZONOSÍTÓ! Minden épület rendelkezik eggyel. NEM UGYANAZ MINT A VIEW ID")]
    public int BuildingID;

    /// <summary>
    /// How much grid space this building occupies, given in width × depth.
    /// </summary>
    [Tooltip("Szemből nézve, szélesség, mélység/hosszúság")]
    public Vector2Int SpaceTaken;

    /// <summary>
    /// Display name of the building.
    /// </summary>
    public string Name;

    /// <summary>
    /// Cost to refill or operate the building.
    /// </summary>
    [Tooltip("Az újratöltés ára")]
    public int Price;

    /// <summary>
    /// The initial cost to purchase the building.
    /// </summary>
    public int BuyingPrice;

    /// <summary>
    /// Represents either hunger restoration or structural capacity (e.g., parking, housing).
    /// </summary>
    [Tooltip("Éhség visszaállítására, vagy a ház/parkoló kapacitása")]
    public int Capacity;

    /// <summary>
    /// Type/classification of the building.
    /// </summary>
    public BuildingType Type;

    /// <summary>
    /// Icon used to visually represent the building in UI.
    /// </summary>
    public Sprite Icon;

    /// <summary>
    /// Relevant diet type, if applicable (e.g., for feeders).
    /// </summary>
    public DietType Diet;

    /// <summary>
    /// Maximum level this building can be upgraded to.
    /// </summary>
    public int MaxLevel;

    /// <summary>
    /// The manager responsible for this building when upgraded.
    /// </summary>
    public ManagerType ToUpgrade;

    /// <summary>
    /// The prefab that visually represents the building in the game world.
    /// </summary>
    [Tooltip("Megjelenítendő objektum")]
    public GameObject BuildingPrefab;

    /// <summary>
    /// Returns the unique data ID used by this building (implementation of IDetails).
    /// </summary>
    public int GetDataID()
    {
        return BuildingID;
    }

    /// <summary>
    /// Returns the string representation of the building's type (for UI display).
    /// </summary>
    public string GetDisplayType()
    {
        return Type.ToString();
    }

    /// <summary>
    /// Returns a detail related to the building's functionality — by default, its Diet.
    /// </summary>
    public OtherDetail GetOtherDetail()
    {
        return new OtherDetail("Diet:", Diet.ToString(), "Occupied space", SpaceTaken.ToString());
    }

    /// <summary>
    /// Returns the cost to purchase the building (used for IDetails).
    /// </summary>
    public int GetPrice()
    {
        return BuyingPrice;
    }
}
