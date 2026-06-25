using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    public static MinigameManager Instance { get; private set; }

    [SerializeField] private ChoiceMinigame choiceMinigame;
    [SerializeField] private SearchMinigame searchMinigame;
    [SerializeField] private PuzzleMinigame puzzleMinigame;
    [SerializeField] private TimingMinigame timingMinigame;
    [SerializeField] private SpecialMinigame specialMinigame;

    public string LastQuestName { get; private set; }
    public bool LastSuccess { get; private set; }
    public int LastGoldReward { get; private set; }
    public int LastRepReward { get; private set; }

    private QuestData _activeQuest;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void StartMinigame(QuestData quest)
    {
        _activeQuest = quest;
        var minigame = GetMinigame(quest.minigameType);
        if (minigame == null)
        {
            Debug.LogWarning($"[MinigameManager] {quest.minigameType} のコンポーネントが未割り当てです");
            return;
        }
        minigame.OnMinigameCompleted += OnCompleted;
        MainSceneController.Instance?.ShowMinigamePanel(quest.minigameType);
        GameManager.Instance.ChangeState(GameState.Minigame);
        minigame.StartMinigame(quest);
    }

    private void OnCompleted(bool success)
    {
        var minigame = GetMinigame(_activeQuest.minigameType);
        if (minigame != null) minigame.OnMinigameCompleted -= OnCompleted;

        LastQuestName  = _activeQuest.questName;
        LastSuccess    = success;
        LastGoldReward = success ? _activeQuest.rewardGold : 0;
        LastRepReward  = success ? _activeQuest.rewardReputation : -5;

        if (success)
            QuestManager.Instance.CompleteQuest(_activeQuest.questId);
        else
            QuestManager.Instance.FailQuest(_activeQuest.questId);

        GameManager.Instance.ChangeState(GameState.Result);
    }

    private BaseMinigame GetMinigame(MinigameType type) => type switch
    {
        MinigameType.Choice  => choiceMinigame,
        MinigameType.Search  => searchMinigame,
        MinigameType.Puzzle  => puzzleMinigame,
        MinigameType.Timing  => timingMinigame,
        MinigameType.Special => specialMinigame,
        _                    => choiceMinigame
    };
}
