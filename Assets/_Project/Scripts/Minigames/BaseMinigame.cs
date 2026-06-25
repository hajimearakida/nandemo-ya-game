using System;
using UnityEngine;

public abstract class BaseMinigame : MonoBehaviour
{
    public event Action<bool> OnMinigameCompleted;

    protected QuestData CurrentQuest { get; private set; }

    public void StartMinigame(QuestData quest)
    {
        CurrentQuest = quest;
        gameObject.SetActive(true);
        StartGame();
    }

    protected abstract void StartGame();

    protected void Complete(bool success)
    {
        gameObject.SetActive(false);
        OnMinigameCompleted?.Invoke(success);
    }
}
