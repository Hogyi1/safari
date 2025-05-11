using System;
using System.Collections.Generic;
using UnityEngine;
using static StructureUIValues;

public abstract class Facility : Structure, IUpgradeable
{
    private int maxLevel;
    private int level;
    private int upgradePrice;
    private ManagerType toupgrade;

    public Facility(BuildingData Data, int iD) : base(iD, Data.Name, Data.icon, Data.type)
    {
        this.maxLevel = Data.level;
        this.level = 1;
        this.toupgrade = Data.toupgrade;
        this.upgradePrice = Data.Price;
    }
    public ManagerType ToUpgrade => toupgrade;
    public int GetUpgradePrice() => upgradePrice;
    public int GetCurrentLevel() => level;
    public bool CanUpgrade() => level < maxLevel;

    public void LevelUp()
    {
        upgradePrice = Mathf.RoundToInt(1.5f * upgradePrice);
        level++;
    }

    public void LevelDown()
    {
        upgradePrice = Mathf.RoundToInt(upgradePrice / 1.5f);
        level--;
    }
}
