using UnityEngine;

public enum LevelStatus { NotPlayed = 0, Lock =  1, Success = 2, Failed = 3 }

public static class LevelProgress
{
    static string Key(int level) => $"level_{level}_status";
    
    public static LevelStatus Get(int level) =>
        (LevelStatus)PlayerPrefs.GetInt(Key(level), 0);

    public static void Set(int level, LevelStatus status)
    {
        if (Get(level) == LevelStatus.Success) return;
        
        PlayerPrefs.SetInt(Key(level), (int)status);
        PlayerPrefs.Save();
    }
    
    public static void Reset(int level)
    {
        PlayerPrefs.DeleteKey(Key(level));
        PlayerPrefs.Save();
    }

    public static void ResetAll(int totalLevels)
    {
        for (int i = 0; i < totalLevels; i++)
            PlayerPrefs.DeleteKey(Key(i));
        PlayerPrefs.Save();
    }
}
