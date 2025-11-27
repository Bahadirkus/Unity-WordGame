using UnityEngine;

/// <summary>
/// ScriptableObject that holds the configuration for a single word level.
/// </summary>
[CreateAssetMenu(fileName = "NewLevel", menuName = "WordGame/Level Data")]
public class LevelDataScriptable : ScriptableObject
{
    [Header("Level Configuration")]
    [Tooltip("Category or hint text displayed at the top of the screen.")]
    public string category;

    [Tooltip("Correct answer for this level.")]
    public string answer;
}
