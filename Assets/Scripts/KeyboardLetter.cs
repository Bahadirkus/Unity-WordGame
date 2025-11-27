using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Represents a single key on the virtual keyboard.
/// Sends its character to the WordGameController when clicked.
/// </summary>
[RequireComponent(typeof(Button))]
public class KeyboardLetter : MonoBehaviour
{
    private Text letterText;
    private Button button;
    private WordGameController controller;

    private char letter;

    private void Awake()
    {
        letterText = GetComponentInChildren<Text>(true);
        button = GetComponent<Button>();
        controller = FindFirstObjectByType<WordGameController>();

        if (letterText == null)
        {
            Debug.LogError($"KeyboardLetter: No Text component found under '{name}'.");
            return;
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);
    }

    private void Start()
    {
        if (!string.IsNullOrEmpty(letterText.text))
        {
            letter = letterText.text[0];
        }
        else
        {
            Debug.LogError($"{name}: Keyboard letter Text is empty.");
        }
    }

    private void OnClick()
    {
        AudioManager.Instance.PlayClick();

        if (controller == null)
            return;

        controller.OnKeyboardLetterPressed(letter);
    }
}
