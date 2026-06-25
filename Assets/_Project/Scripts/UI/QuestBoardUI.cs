using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestBoardUI : MonoBehaviour
{
    [SerializeField] private Transform questListContainer;
    [SerializeField] private GameObject questEntryPrefab;
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI reputationText;
    [SerializeField] private Button endDayButton;

    private QuestData _pendingQuest;

    void Start()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged += OnStateChanged;
        if (EconomyManager.Instance != null)
            EconomyManager.Instance.OnValuesChanged += Refresh;
        if (DialogueSystem.Instance != null)
            DialogueSystem.Instance.OnDialogueCompleted += OnDialogueCompleted;
        endDayButton.onClick.AddListener(() => GameManager.Instance.AdvanceDay());
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged -= OnStateChanged;
        if (EconomyManager.Instance != null)
            EconomyManager.Instance.OnValuesChanged -= Refresh;
        if (DialogueSystem.Instance != null)
            DialogueSystem.Instance.OnDialogueCompleted -= OnDialogueCompleted;
    }

    private void OnStateChanged(GameState state)
    {
        gameObject.SetActive(state == GameState.QuestBoard);
        if (state == GameState.QuestBoard) Refresh();
    }

    public void Refresh()
    {
        if (GameManager.Instance?.CurrentSave == null) return;

        var save = GameManager.Instance.CurrentSave;
        dayText.text = $"{save.currentDay}日目 / 60日";
        goldText.text = $"所持金: {save.gold}G";
        reputationText.text = $"評判: {save.reputation}";

        foreach (Transform child in questListContainer)
            Destroy(child.gameObject);

        var quests = QuestManager.Instance.GetAvailableQuests();
        foreach (var quest in quests)
        {
            var entry = Instantiate(questEntryPrefab, questListContainer);
            var label = entry.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null)
                label.text = $"[{Stars(quest.difficulty)}]  {quest.questName}\n<size=70%>{quest.description}</size>";

            var captured = quest;
            var btn = entry.GetComponent<Button>();
            if (btn != null)
                btn.onClick.AddListener(() => SelectQuest(captured));
        }
    }

    private void SelectQuest(QuestData quest)
    {
        _pendingQuest = quest;

        if (quest.dialogueData != null)
        {
            DialogueSystem.Instance.StartDialogue(quest.dialogueData);
            GameManager.Instance.ChangeState(GameState.Dialogue);
        }
        else
        {
            MinigameManager.Instance.StartMinigame(quest);
        }
    }

    private void OnDialogueCompleted(string resultFlag)
    {
        if (_pendingQuest == null) return;

        if (resultFlag == "accept" || string.IsNullOrEmpty(resultFlag))
        {
            var quest = _pendingQuest;
            _pendingQuest = null;
            MinigameManager.Instance.StartMinigame(quest);
        }
        else
        {
            _pendingQuest = null;
            GameManager.Instance.ChangeState(GameState.QuestBoard);
        }
    }

    private string Stars(int difficulty) => difficulty switch
    {
        1 => "★☆☆",
        2 => "★★☆",
        3 => "★★★",
        _ => "☆☆☆"
    };
}
