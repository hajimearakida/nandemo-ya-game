using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    public static MinigameManager Instance { get; private set; }

    [SerializeField] private ChoiceMinigame choiceMinigame;
    [SerializeField] private SearchMinigame searchMinigame;
    [SerializeField] private PuzzleMinigame puzzleMinigame;
    [SerializeField] private TimingMinigame timingMinigame;
    [SerializeField] private SpecialMinigame specialMinigame;

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
        minigame.OnMinigameCompleted += OnCompleted;
        minigame.StartMinigame(quest);
        GameManager.Instance.ChangeState(GameState.Minigame);
    }

    private void OnCompleted(bool success)
    {
        GetMinigame(_activeQuest.minigameType).OnMinigameCompleted -= OnCompleted;

        if (success)
            QuestManager.Instance.CompleteQuest(_activeQuest.questId);
        else
            QuestManager.Instance.FailQuest(_activeQuest.questId);
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
