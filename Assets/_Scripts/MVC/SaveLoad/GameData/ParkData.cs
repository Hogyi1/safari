using UnityEngine;

[System.Serializable]
public class ParkData
{
    public string parkName;
    public DifficultyEnum difficulty;

    public ParkData()
    {
        parkName = "default";
        difficulty = DifficultyEnum.EASY;
    }

}
