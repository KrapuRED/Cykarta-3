using UnityEngine;

[System.Serializable]
public enum DialogueType { Before = 0, After = 1 }

public static class DialogueProgress 
{
    private static string Key(int level, DialogueType type) =>
        $"dialogue_{level}_{type}_played";
    
    public static bool IsPlayed(int level, DialogueType type) =>
        PlayerPrefs.GetInt(Key(level, type), 0) == 1;

    public static void MarkPlayed(int level, DialogueType type)
    {
        PlayerPrefs.SetInt(Key(level, type), 1);
        PlayerPrefs.Save();
    }

    public static void Reset(int level)
    {
        PlayerPrefs.DeleteKey(Key(level, DialogueType.Before));
        PlayerPrefs.DeleteKey(Key(level, DialogueType.After));
        PlayerPrefs.Save();
    }

    public static void ResetAll(int totalLevels)
    {
        for (int i = 0; i < totalLevels; i++)
        {
            PlayerPrefs.DeleteKey(Key(i, DialogueType.Before));
            PlayerPrefs.DeleteKey(Key(i, DialogueType.After));
        }
        PlayerPrefs.Save();
    }
}
