using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A single answer slot in the answer row.
/// Holds one character and can be locked by a hint.
/// </summary>
[RequireComponent(typeof(Button))]
public class AnswerSlot : MonoBehaviour
{
    [SerializeField] private Text letterText;

    private Button button;
    private WordGameController controller;

    private int index;
    private char currentLetter;
    private bool isLocked;

    private void Awake()
    {
        button = GetComponent<Button>();

        if (letterText == null)
            letterText = GetComponentInChildren<Text>(true);
    }

    public void Initialize(WordGameController owner, int slotIndex)
    {
        controller = owner;
        index = slotIndex;
        isLocked = false;
        currentLetter = ' ';

        if (letterText != null)
        {
            letterText.text = string.Empty;
            letterText.color = Color.black;
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        AudioManager.Instance.PlayClick();

        if (isLocked || controller == null)
            return;

        controller.OnAnswerSlotClicked(this);
    }

    public void SetLetter(char c)
    {
        currentLetter = c;

        if (letterText != null)
            letterText.text = c.ToString();
    }

    public void Clear()
    {
        if (isLocked)
            return;

        currentLetter = ' ';

        if (letterText != null)
        {
            letterText.text = string.Empty;
            letterText.color = Color.black;
        }
    }

    public void SetHintAndLock()
    {
        isLocked = true;

        if (letterText != null)
            letterText.color = Color.red;
    }

    public bool IsFilled()
    {
        return letterText != null && !string.IsNullOrEmpty(letterText.text);
    }

    public bool IsLocked() => isLocked;
    public char GetLetter() => currentLetter;
    public int GetIndex() => index;
}
