using UnityEngine;

public class MainSceneController : MonoBehaviour
{
    public static MainSceneController Instance { get; private set; }

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

    void Awake()
    {
        if (Instance != null) { Destroy(this); return; }
        Instance = this;
    }

    void Start()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged += OnStateChanged;
    }

    void OnDestroy()
    {
        Instance = null;
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged -= OnStateChanged;
    }

    private void OnStateChanged(GameState state)
    {
        HideAll();
        switch (state)
        {
            case GameState.DayStart:   panelDayStart?.SetActive(true);   break;
            case GameState.QuestBoard: panelQuestBoard?.SetActive(true);  break;
            case GameState.Dialogue:   panelDialogue?.SetActive(true);    break;
            case GameState.Result:     panelResult?.SetActive(true);      break;
            case GameState.Shop:       panelShop?.SetActive(true);        break;
            case GameState.Ending:     panelEnding?.SetActive(true);      break;
            case GameState.GameOver:   panelGameOver?.SetActive(true);    break;
            // Minigame: handled by ShowMinigamePanel
        }
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
        switch (type)
        {
            case MinigameType.Choice:  panelChoice?.SetActive(true);  break;
            case MinigameType.Search:  panelSearch?.SetActive(true);  break;
            case MinigameType.Puzzle:  panelPuzzle?.SetActive(true);  break;
            case MinigameType.Timing:  panelTiming?.SetActive(true);  break;
            case MinigameType.Special: panelSpecial?.SetActive(true); break;
        }
    }
}
