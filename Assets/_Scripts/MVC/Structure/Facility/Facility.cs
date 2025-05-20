using UnityEngine;

/// <summary>
/// Abstract base class for facility-type structures that can be upgraded,
/// such as garages or ranger houses. Implements upgrade logic and save support.
/// </summary>
public abstract class Facility : Structure, IUpgradeable
{
    /// <summary>
    /// The maximum level this facility can be upgraded to.
    /// </summary>
    private int maxLevel;

    /// <summary>
    /// The current level of the facility.
    /// </summary>
    private int level;

    /// <summary>
    /// The current cost to upgrade this facility.
    /// </summary>
    private int upgradePrice;

    /// <summary>
    /// The manager type this facility supports (e.g., Vehicle, Ranger).
    /// </summary>
    private ManagerType toupgrade;

    /// <summary>
    /// The capacity value this facility provides (e.g., for parking).
    /// </summary>
    private int capacity;

    /// <summary>
    /// The amount to upgrade
    /// </summary>
    private int upgradeAmount;

    /// <summary>
    /// Initializes a new facility using static building data and an assigned ID.
    /// </summary>
    /// <param name="data">The building template data.</param>
    /// <param name="iD">The unique ID of this facility instance.</param>
    public Facility(BuildingData data, int iD) : base(iD, data.Name, data.Icon, data.Type)
    {
        maxLevel = data.MaxLevel;
        level = 1;
        toupgrade = data.ToUpgrade;
        upgradePrice = data.Price;
        capacity = data.Capacity;
        upgradeAmount = data.UpgradeAmount;
    }


    /// <summary>
    /// Constructs a facility structure from saved state and template data.
    /// </summary>
    /// <param name="data">The base building template (e.g., parking, ranger house).</param>
    /// <param name="saveData">Saved dynamic state of the facility.</param>
    public Facility(BuildingData data, StructureSaveData saveData)
        : base(saveData.UniqueID, data.Name, data.Icon, data.Type)
    {
        level = saveData.Level;
        maxLevel = data.MaxLevel;
        toupgrade = saveData.FacilityType;
        upgradePrice = saveData.UpgradePrice;
        capacity = saveData.FacilityCapacity;
    }


    /// <summary>
    /// Gets the manager type this facility upgrades (Vehicle, Ranger, etc.).
    /// </summary>
    public ManagerType ToUpgrade => toupgrade;


    /// <summary>
    /// Gets the current upgrade price.
    /// </summary>
    public int GetUpgradePrice() => upgradePrice;


    /// <summary>
    /// Gets the current upgrade level.
    /// </summary>
    public int GetCurrentLevel() => level;


    /// <summary>
    /// Returns true if this facility can be upgraded further.
    /// </summary>
    public bool CanUpgrade() => level < maxLevel;


    /// <summary>
    /// Gets the current capacity value this facility provides.
    /// </summary>
    public int Capacity => capacity;


    /// <summary>
    /// Gets the upgrade amount
    /// </summary>
    public int UpgradeAmount => upgradeAmount;


    /// <summary>
    /// Sets the capacity
    /// </summary>
    /// <param name="amount"></param>
    public void SetCapacity(int amount) => capacity = amount;


    /// <summary>
    /// Increases the level of the facility and updates capacity and upgrade price.
    /// </summary>
    /// <param name="amount">The amount of capacity to add.</param>
    public void LevelUp(int amount)
    {
        upgradePrice = Mathf.RoundToInt(1.5f * upgradePrice);
        capacity += amount;
        level++;
    }


    /// <summary>
    /// Decreases the level of the facility and reduces capacity and upgrade price.
    /// </summary>
    /// <param name="amount">The amount of capacity to subtract.</param>
    public void LevelDown(int amount)
    {
        upgradePrice = Mathf.RoundToInt(upgradePrice / 1.5f);
        capacity -= amount;
        level--;
    }


    /// <summary>
    /// Serializes the facility state into a save data object.
    /// </summary>
    public override StructureSaveData GetSaveData()
    {
        return new StructureSaveData(ID, buildingType)
        {
            Level = level,
            FacilityCapacity = capacity,
            FacilityType = toupgrade,
            UpgradePrice = upgradePrice,
        };
    }
}
