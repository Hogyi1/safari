using UnityEngine;

public enum DifficultyEnum
{
    EASY,
    NORMAL,
    HARD
}

public static class DifficultyExtensions
{
    public static int GetMultiplier(this DifficultyEnum difficulty)
    {
        return difficulty switch
        {
            DifficultyEnum.EASY => 1,
            DifficultyEnum.NORMAL => 2,
            DifficultyEnum.HARD => 3,
            _ => 1
        };
    }
}
