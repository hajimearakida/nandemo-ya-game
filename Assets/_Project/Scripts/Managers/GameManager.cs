using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    DayStart,
    QuestBoard,
    Dialogue,
    Minigame,
    Result,
    Shop,
    DayEnd,
    GameOver,
    Ending
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState CurrentState { get; private set; }
    public SaveData CurrentSave { get; private set; }

    public event Action<GameState> OnStateChanged;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartNewGame()
    {
        CurrentSave = new SaveData();
        SaveSystem.Save(CurrentSave);
        ChangeState(GameState.DayStart);
    }

    public void LoadGame()
    {
        CurrentSave = SaveSystem.Load();
        ChangeState(GameState.DayStart);
    }

    public void ChangeState(GameState newState)
    {
        CurrentState = newState;
        OnStateChanged?.Invoke(newState);
    }

    public void AdvanceDay()
    {
        CurrentSave.currentDay++;
        SaveSystem.Save(CurrentSave);

        if (CurrentSave.currentDay > 60)
        {
            ChangeState(GameState.Ending);
            return;
        }

        if (CurrentSave.reputation <= 0)
        {
            ChangeState(GameState.GameOver);
            return;
        }

        ChangeState(GameState.DayStart);
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
