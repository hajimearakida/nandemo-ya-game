using System.Collections.Generic;
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

    void OnEnable()
    {
        Refresh();
        GameManager.Instance.OnStateChanged += OnStateChanged;
        EconomyManager.Instance.OnValuesChanged += Refresh;
    }

    void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged -= OnStateChanged;
        if (EconomyManager.Instance != null)
            EconomyManager.Instance.OnValuesChanged -= Refresh;
    }

    void Start()
    {
        endDayButton.onClick.AddListener(() => GameManager.Instance.AdvanceDay());
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
            label.text = $"[{Stars(quest.difficulty)}]  {quest.questName}\n<size=70%>{quest.description}</size>";

            var captured = quest;
            entry.GetComponent<Button>().onClick.AddListener(() => SelectQuest(captured));
        }
    }

    private void SelectQuest(QuestData quest)
    {
        GameManager.Instance.ChangeState(GameState.Dialogue);
    }

    private string Stars(int difficulty) => difficulty switch
    {
        1 => "★☆☆",
        2 => "★★☆",
        3 => "★★★",
        _ => "☆☆☆"
    };
}
