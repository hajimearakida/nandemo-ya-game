using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI speakerText;
    [SerializeField] private TextMeshProUGUI bodyText;
    [SerializeField] private Button nextButton;
    [SerializeField] private GameObject choicesArea;
    [SerializeField] private Button[] choiceButtons;
    [SerializeField] private TextMeshProUGUI[] choiceLabels;

    void Start()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged += OnStateChanged;
        if (DialogueSystem.Instance != null)
        {
            DialogueSystem.Instance.OnLineDisplayed += ShowLine;
            DialogueSystem.Instance.OnChoicesDisplayed += ShowChoices;
        }
        if (nextButton != null)
            nextButton.onClick.AddListener(() => DialogueSystem.Instance?.Next());
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged -= OnStateChanged;
        if (DialogueSystem.Instance != null)
        {
            DialogueSystem.Instance.OnLineDisplayed -= ShowLine;
            DialogueSystem.Instance.OnChoicesDisplayed -= ShowChoices;
        }
    }

    private void OnStateChanged(GameState state)
    {
        gameObject.SetActive(state == GameState.Dialogue);
    }

    private void ShowLine(DialogueLine line)
    {
        var charData = CharacterManager.Instance?.GetData(line.characterId);
        if (speakerText != null)
            speakerText.text = charData != null ? charData.characterName : line.characterId;
        if (bodyText != null)
            bodyText.text = line.text;

        bool hasChoices = line.HasChoices;
        if (nextButton != null)   nextButton.gameObject.SetActive(!hasChoices);
        if (choicesArea != null)  choicesArea.SetActive(false);
    }

    private void ShowChoices(DialogueChoice[] choices)
    {
        if (nextButton != null)  nextButton.gameObject.SetActive(false);
        if (choicesArea != null) choicesArea.SetActive(true);

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            bool active = i < choices.Length;
            choiceButtons[i].gameObject.SetActive(active);
            if (!active) continue;

            choiceLabels[i].text = choices[i].label;
            int captured = i;
            choiceButtons[i].onClick.RemoveAllListeners();
            choiceButtons[i].onClick.AddListener(() => DialogueSystem.Instance.SelectChoice(captured));
        }
    }
}
