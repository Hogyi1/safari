using UnityEngine;

[System.Serializable]
public class LevelSaveData 
{

    public int currentLevel;
    public int currentExp;
    public float progress;
    public bool isMaxLevel;


    public LevelSaveData()
    {
        this.currentLevel = 1;
        this.currentExp = 0;
        this.progress = 0;
        this.isMaxLevel = false;
    }


}
