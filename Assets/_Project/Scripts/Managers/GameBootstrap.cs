using UnityEngine;
using System.Collections;

public class GameBootstrap : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private EconomyManager economyManager;
    [SerializeField] private QuestManager questManager;
    [SerializeField] private CharacterManager characterManager;
    [SerializeField] private MinigameManager minigameManager;
    [SerializeField] private DialogueSystem dialogueSystem;
    [SerializeField] private ShopSystem shopSystem;
    [SerializeField] private AchievementManager achievementManager;
    [SerializeField] private EndingManager endingManager;

    [Header("Start Mode")]
    [SerializeField] private bool autoStartNewGame = true;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (autoStartNewGame)
            StartCoroutine(DelayedStart());
    }

    private IEnumerator DelayedStart()
    {
        yield return null; // wait one frame so all Start() subscriptions are registered
        if (SaveSystem.HasSaveData())
            gameManager.LoadGame();
        else
            gameManager.StartNewGame();
    }
}
