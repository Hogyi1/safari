using UnityEngine;

public abstract class Facility : Structure, IUpgradeable
{
    private int maxLevel;
    private int level;
    private int upgradePrice;
    private ManagerType toupgrade;
    private int capacity;

    public Facility(BuildingData Data, int iD) : base(iD, Data.Name, Data.Icon, Data.Type)
    {
        this.maxLevel = Data.MaxLevel;
        this.level = 1;
        this.toupgrade = Data.ToUpgrade;
        this.upgradePrice = Data.Price;
        this.capacity = Data.Capacity;
    }
    public ManagerType ToUpgrade => toupgrade;
    public int GetUpgradePrice() => upgradePrice;
    public int GetCurrentLevel() => level;
    public bool CanUpgrade() => level < maxLevel;

    public void LevelUp(int amount)
    {
        upgradePrice = Mathf.RoundToInt(1.5f * upgradePrice);
        capacity += amount;
        level++;
    }

    public void LevelDown(int amount)
    {
        upgradePrice = Mathf.RoundToInt(upgradePrice / 1.5f);
        capacity -= amount;
        level--;
    }
    public int Capacity => capacity;

}
