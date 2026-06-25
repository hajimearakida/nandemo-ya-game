using UnityEngine;

public class MainSceneController : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject panelDayStart;
    [SerializeField] private GameObject panelQuestBoard;
    [SerializeField] private GameObject panelDialogue;
    [SerializeField] private GameObject panelResult;
    [SerializeField] private GameObject panelShop;
    [SerializeField] private GameObject panelEnding;
    [SerializeField] private GameObject panelGameOver;

    [Header("Minigame Panels")]
    [SerializeField] private GameObject panelChoice;
    [SerializeField] private GameObject panelSearch;
    [SerializeField] private GameObject panelPuzzle;
    [SerializeField] private GameObject panelTiming;
    [SerializeField] private GameObject panelSpecial;

    void Start()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged += OnStateChanged;
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged -= OnStateChanged;
    }

    private void OnStateChanged(GameState state)
    {
        HideAll();
        var target = state switch
        {
            GameState.DayStart  => panelDayStart,
            GameState.QuestBoard => panelQuestBoard,
            GameState.Dialogue  => panelDialogue,
            GameState.Result    => panelResult,
            GameState.Shop      => panelShop,
            GameState.Ending    => panelEnding,
            GameState.GameOver  => panelGameOver,
            _                   => null
        };
        target?.SetActive(true);
    }

    private void HideAll()
    {
        panelDayStart?.SetActive(false);
        panelQuestBoard?.SetActive(false);
        panelDialogue?.SetActive(false);
        panelResult?.SetActive(false);
        panelShop?.SetActive(false);
        panelEnding?.SetActive(false);
        panelGameOver?.SetActive(false);
        panelChoice?.SetActive(false);
        panelSearch?.SetActive(false);
        panelPuzzle?.SetActive(false);
        panelTiming?.SetActive(false);
        panelSpecial?.SetActive(false);
    }

    public void ShowMinigamePanel(MinigameType type)
    {
        HideAll();
        var target = type switch
        {
            MinigameType.Choice  => panelChoice,
            MinigameType.Search  => panelSearch,
            MinigameType.Puzzle  => panelPuzzle,
            MinigameType.Timing  => panelTiming,
            MinigameType.Special => panelSpecial,
            _                    => panelChoice
        };
        target?.SetActive(true);
    }
}
