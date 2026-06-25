using System.Collections.Generic;
using UnityEngine;

#if STEAMWORKS_NET
using Steamworks;
#endif

public static class AchievementId
{
    public const string AllFriends        = "ACH_ALL_FRIENDS";
    public const string NearBankrupt      = "ACH_NEAR_BANKRUPT";
    public const string SpeedMaster       = "ACH_SPEED_MASTER";
    public const string WeirdSolution     = "ACH_WEIRD_SOLUTION";
    public const string HiddenClient      = "ACH_HIDDEN_CLIENT";
    public const string PerfectFinalDay   = "ACH_PERFECT_FINAL_DAY";
}

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Unlock(string achievementId)
    {
        var save = GameManager.Instance.CurrentSave;
        if (save.unlockedAchievementIds.Contains(achievementId)) return;

        save.unlockedAchievementIds.Add(achievementId);
        SaveSystem.Save(save);

#if STEAMWORKS_NET
        if (SteamManager.Initialized)
        {
            SteamUserStats.SetAchievement(achievementId);
            SteamUserStats.StoreStats();
        }
#else
        Debug.Log($"[Achievement] Unlocked: {achievementId}");
#endif
    }

    public bool IsUnlocked(string achievementId)
    {
        return GameManager.Instance.CurrentSave.unlockedAchievementIds.Contains(achievementId);
    }

    public void CheckAutoAchievements()
    {
        var save = GameManager.Instance.CurrentSave;

        if (CharacterManager.Instance.AllMainRelationsMaxed())
            Unlock(AchievementId.AllFriends);

        if (save.reputation <= 15)
            Unlock(AchievementId.NearBankrupt);

        if (save.unlockedAchievementIds.Contains(AchievementId.HiddenClient)
            && CharacterManager.Instance.AllMainRelationsMaxed()
            && save.currentDay == 60)
            Unlock(AchievementId.PerfectFinalDay);
    }
}
