using System;
using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance { get; private set; }

    private const int GameOverReputationThreshold = 10;

    public event Action OnValuesChanged;

    public int Gold => GameManager.Instance.CurrentSave.gold;
    public int Reputation => GameManager.Instance.CurrentSave.reputation;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void AddGold(int amount)
    {
        GameManager.Instance.CurrentSave.gold = Mathf.Max(0, Gold + amount);
        OnValuesChanged?.Invoke();
    }

    public bool SpendGold(int amount)
    {
        if (Gold < amount) return false;
        GameManager.Instance.CurrentSave.gold -= amount;
        OnValuesChanged?.Invoke();
        return true;
    }

    public void AddReputation(int delta)
    {
        GameManager.Instance.CurrentSave.reputation = Mathf.Clamp(Reputation + delta, 0, 100);
        OnValuesChanged?.Invoke();

        if (Reputation <= GameOverReputationThreshold)
            GameManager.Instance.ChangeState(GameState.GameOver);
    }
}
