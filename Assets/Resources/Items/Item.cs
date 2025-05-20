using UnityEngine;

/// <summary>
/// Represents a single item that can exist in the inventory system.
/// Stores metadata like name, category, lock state, and a reference to its detailed data object.
/// </summary>
[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    /// <summary>
    /// Globally unique identifier of the item.
    /// </summary>
    public int ID;

    /// <summary>
    /// Display name of the item.
    /// </summary>
    public string ItemName;

    /// <summary>
    /// Category classification of the item (e.g., structure, vegetation).
    /// Used for UI filtering or sorting.
    /// </summary>
    public Category Category;

    /// <summary>
    /// Indicates whether the item is currently locked or unlocked for use.
    /// </summary>
    public LockState LockState;

    /// <summary>
    /// Sprite used to visually represent the item in the UI.
    /// </summary>
    public Sprite Image;

    /// <summary>
    /// Reference to the item's concrete data object, implementing IDetails.
    /// </summary>
    public ScriptableObject ItemData;

    /// <summary>
    /// The item is manually placed?
    /// </summary>
    public bool Placeable;

    /// <summary>
    /// Calculates the item's selling price, which is 50% of its original buy price.
    /// </summary>
    /// <returns>The integer value of the selling price.</returns>
    public int CalculateSellingPrice() => Mathf.RoundToInt(((IDetails)ItemData).GetPrice() * 0.5f);

    /// <summary>
    /// Returns the item's type or classification as a string (used in UI).
    /// </summary>
    public string GetDisplayType() => ((IDetails)ItemData).GetDisplayType();

    /// <summary>
    /// Returns the placement ID associated with the item’s data (e.g., structure or animal ID).
    /// </summary>
    public int GetPlacementID() => ((IDetails)ItemData).GetDataID();

    /// <summary>
    /// Returns the full price of the item in in-game currency.
    /// </summary>
    public int GetPrice() => (Mathf.RoundToInt(((IDetails)ItemData).GetPrice() * DifficultyExtensions.GetMultiplier(Park.Instance.Difficulty)));

    /// <summary>
    /// Returns an additional detail about the item (e.g., diet, lifespan, area).
    /// </summary>
    public OtherDetail GetOtherDetail() => ((IDetails)ItemData).GetOtherDetail();
}

/// <summary>
/// Interface implemented by all item data sources (building, vehicle, animal, etc.)
/// Allows unified access to display and pricing data.
/// </summary>
public interface IDetails
{
    /// <summary>
    /// Returns the display-friendly type or classification of the item.
    /// </summary>
    string GetDisplayType();

    /// <summary>
    /// Returns the unique data ID associated with the item.
    /// </summary>
    int GetDataID();

    /// <summary>
    /// Returns the item's full price.
    /// </summary>
    int GetPrice();

    /// <summary>
    /// Returns one supplementary detail about the item, such as diet or area used.
    /// </summary>
    OtherDetail GetOtherDetail();
}

/// <summary>
/// Struct for representing a key-value style detail to be displayed in UI (e.g., "Capacity: 10").
/// </summary>
public struct OtherDetail
{
    public OtherDetail(string title, string info, string title2, string info2)
    {
        Title = title;
        Info = info;
        Title2 = title2;
        Info2 = info2;
    }

    /// <summary>
    /// The label of the detail (e.g., "Diet", "Size", "Lifespan").
    /// </summary>
    public string Title;

    /// <summary>
    /// The actual value/info associated with the detail.
    /// </summary>
    public string Info;

    /// <summary>
    /// The label of the detail (e.g., "Diet", "Size", "Lifespan").
    /// </summary>
    public string Title2;

    /// <summary>
    /// The actual value/info associated with the detail.
    /// </summary>
    public string Info2;
}
