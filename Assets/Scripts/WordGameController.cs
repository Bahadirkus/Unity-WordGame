using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Central controller for the word puzzle:
/// - loads and initializes levels
/// - manages answer slots
/// - handles keyboard input, delete, hint and submit
/// - saves and restores progress.
/// </summary>
public class WordGameController : MonoBehaviour
{
    [Header("Prefabs & Layout")]
    [SerializeField] private GameObject answerSlotPrefab;
    [SerializeField] private Transform answerContainer;

    [Header("UI")]
    [SerializeField] private Text categoryText;
    [SerializeField] private Text levelText;
    [SerializeField] private Text feedbackText;

    private LevelDataScriptable[] levels;
    private int currentLevelIndex;
    private string currentAnswer;
    private readonly List<AnswerSlot> answerSlots = new List<AnswerSlot>();

    private void Start()
    {
        LoadLevels();

        currentLevelIndex = SaveManager.LoadProgress();
        if (levels.Length > 0 && currentLevelIndex >= levels.Length)
            currentLevelIndex = 0;

        InitializeLevel(currentLevelIndex);
    }

    /// <summary>
    /// Loads all level assets from Resources/Levels and orders them by numeric name.
    /// </summary>
    private void LoadLevels()
    {
        levels = Resources.LoadAll<LevelDataScriptable>("Levels");

        levels = levels
            .OrderBy(x =>
            {
                if (int.TryParse(x.name, out int n))
                    return n;
                return int.MaxValue;
            })
            .ToArray();
    }

    /// <summary>
    /// Sets up a given level: clears previous answer slots, spawns new ones,
    /// and updates the UI labels.
    /// </summary>
    private void InitializeLevel(int levelIndex)
    {
        if (levels == null || levels.Length == 0)
        {
            Debug.LogError("No level data found. Please add LevelDataScriptable assets under Resources/Levels.");
            return;
        }

        if (levelIndex >= levels.Length)
            levelIndex = 0;

        foreach (Transform child in answerContainer)
            Destroy(child.gameObject);

        answerSlots.Clear();
        ClearFeedbackImmediate();

        LevelDataScriptable level = levels[levelIndex];

        currentAnswer = level.answer
            .Replace(" ", string.Empty)
            .ToUpperInvariant();

        if (categoryText != null)
            categoryText.text = level.category;

        if (levelText != null)
            levelText.text = $"Level {level.name}";

        for (int i = 0; i < currentAnswer.Length; i++)
        {
            GameObject slotObj = Instantiate(answerSlotPrefab, answerContainer);
            AnswerSlot slot = slotObj.GetComponent<AnswerSlot>();

            if (slot == null)
            {
                Debug.LogError("AnswerSlotPrefab is missing the AnswerSlot component.");
                continue;
            }

            slot.Initialize(this, i);
            answerSlots.Add(slot);
        }
    }

    // ----------- Keyboard & slot interaction -----------

    /// <summary>
    /// Called by KeyboardLetter when a key is pressed.
    /// Fills the first empty answer slot with the given character.
    /// </summary>
    public void OnKeyboardLetterPressed(char letter)
    {
        ClearFeedbackImmediate();

        AnswerSlot empty = GetFirstEmptyAnswerSlot();
        if (empty != null)
            empty.SetLetter(letter);
    }

    /// <summary>
    /// Called by an AnswerSlot when it is clicked.
    /// Clears the slot so the player can fix mistakes directly.
    /// </summary>
    public void OnAnswerSlotClicked(AnswerSlot slot)
    {
        ClearFeedbackImmediate();
        slot.Clear();
    }

    // ---------------- UI buttons ----------------

    public void SubmitAnswer()
    {
        string playerAnswer = string.Empty;

        foreach (AnswerSlot slot in answerSlots)
            playerAnswer += slot.GetLetter();

        if (playerAnswer == currentAnswer)
        {
            ShowFeedback("PERFECT!", Color.green);
            SaveManager.SaveProgress(currentLevelIndex + 1);
            Invoke(nameof(GoToNextLevel), 1.2f);
        }
        else
        {
            ShowFeedback("TRY AGAIN", Color.red);
            Invoke(nameof(ClearFeedbackImmediate), 1.2f);
        }
    }

    public void DeleteLastLetter()
    {
        ClearFeedbackImmediate();

        for (int i = answerSlots.Count - 1; i >= 0; i--)
        {
            if (answerSlots[i].IsFilled() && !answerSlots[i].IsLocked())
            {
                answerSlots[i].Clear();
                return;
            }
        }
    }

    public void GiveHint()
    {
        var candidates = new List<int>();

        for (int i = 0; i < answerSlots.Count; i++)
        {
            AnswerSlot slot = answerSlots[i];
            char correct = currentAnswer[i];

            if (!slot.IsLocked() &&
               (!slot.IsFilled() || slot.GetLetter() != correct))
            {
                candidates.Add(i);
            }
        }

        if (candidates.Count == 0)
            return;

        int randomIndex = candidates[Random.Range(0, candidates.Count)];
        AnswerSlot target = answerSlots[randomIndex];

        target.SetLetter(currentAnswer[randomIndex]);
        target.SetHintAndLock();
    }

    private void GoToNextLevel()
    {
        currentLevelIndex++;
        if (currentLevelIndex >= levels.Length)
            currentLevelIndex = 0;

        InitializeLevel(currentLevelIndex);
    }

    // --------------- Helpers ---------------

    private AnswerSlot GetFirstEmptyAnswerSlot()
    {
        foreach (AnswerSlot slot in answerSlots)
        {
            if (!slot.IsFilled())
                return slot;
        }

        return null;
    }

    private void ShowFeedback(string message, Color color)
    {
        if (feedbackText == null)
            return;

        feedbackText.gameObject.SetActive(true);
        feedbackText.text = message;
        feedbackText.color = color;
    }

    private void ClearFeedbackImmediate()
    {
        if (feedbackText == null)
            return;

        feedbackText.text = string.Empty;
    }
}
