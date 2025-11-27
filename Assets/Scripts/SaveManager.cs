using UnityEngine;

/// <summary>
/// Helper for saving and loading the current level index using PlayerPrefs.
/// </summary>
public static class SaveManager
{
    private const string LevelKey = "CurrentLevel";

    public static void SaveProgress(int levelIndex)
    {
        PlayerPrefs.SetInt(LevelKey, levelIndex);
        PlayerPrefs.Save();
    }

    public static int LoadProgress()
    {
        return PlayerPrefs.GetInt(LevelKey, 0);
    }

    public static void ClearData()
    {
        PlayerPrefs.DeleteKey(LevelKey);
    }
}
