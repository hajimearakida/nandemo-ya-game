using UnityEngine;

public enum EndingType
{
    HereIsHome,
    BackToTravel,
    Bankrupt,
    MysteriousClient,
    PerfectHandyman
}

public class EndingManager : MonoBehaviour
{
    public static EndingManager Instance { get; private set; }

    private static readonly string[] TamaQuestIds  = { "quest_40", "quest_41", "quest_42", "quest_48", "quest_49" };
    private static readonly string[] RokkaQuestIds = { "quest_43", "quest_44", "quest_45", "quest_50" };

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public EndingType DetermineEnding()
    {
        var save = GameManager.Instance.CurrentSave;

        if (AllQuestsCompleted(save) && save.unlockedAchievementIds.Count >= 5)
            return EndingType.PerfectHandyman;

        if (HiddenRouteCompleted(save))
            return EndingType.MysteriousClient;

        if (CharacterManager.Instance.AllMainRelationsMaxed() && save.reputation >= 80)
            return EndingType.HereIsHome;

        if (save.reputation >= 40)
            return EndingType.BackToTravel;

        return EndingType.Bankrupt;
    }

    private bool AllQuestsCompleted(SaveData save)
    {
        foreach (var entry in save.questEntries)
        {
            if (entry.state != QuestState.Completed) return false;
        }
        return save.questEntries.Count >= 50;
    }

    private bool HiddenRouteCompleted(SaveData save)
    {
        foreach (var id in TamaQuestIds)
        {
            var entry = save.questEntries.Find(e => e.questId == id);
            if (entry?.state != QuestState.Completed) return false;
        }
        foreach (var id in RokkaQuestIds)
        {
            var entry = save.questEntries.Find(e => e.questId == id);
            if (entry?.state != QuestState.Completed) return false;
        }
        return true;
    }
}
