using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [SerializeField] private QuestData[] allQuests;

    public event Action OnQuestsUpdated;

    private Dictionary<string, QuestData> _questDict = new();

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;

        foreach (var q in allQuests)
            _questDict[q.questId] = q;
    }

    public List<QuestData> GetAvailableQuests()
    {
        var save = GameManager.Instance.CurrentSave;
        return allQuests
            .Where(q => GetQuestState(q.questId) == QuestState.Available && IsUnlocked(q, save))
            .ToList();
    }

    public QuestState GetQuestState(string questId)
    {
        var save = GameManager.Instance.CurrentSave;
        var entry = save.questEntries.Find(e => e.questId == questId);
        if (entry != null) return entry.state;

        if (_questDict.TryGetValue(questId, out var quest))
            return quest.unlockCondition.HasConditions ? QuestState.Locked : QuestState.Available;

        return QuestState.Locked;
    }

    public void RefreshAvailability()
    {
        var save = GameManager.Instance.CurrentSave;
        bool changed = false;

        foreach (var quest in allQuests)
        {
            if (GetQuestState(quest.questId) == QuestState.Locked && IsUnlocked(quest, save))
            {
                SetQuestState(quest.questId, QuestState.Available, save);
                changed = true;
            }
        }

        if (changed) OnQuestsUpdated?.Invoke();
    }

    public void CompleteQuest(string questId)
    {
        var save = GameManager.Instance.CurrentSave;
        SetQuestState(questId, QuestState.Completed, save);

        if (_questDict.TryGetValue(questId, out var quest))
        {
            EconomyManager.Instance.AddGold(quest.rewardGold);
            EconomyManager.Instance.AddReputation(quest.rewardReputation);
            if (!string.IsNullOrEmpty(quest.characterId))
                CharacterManager.Instance.AddRelation(quest.characterId, quest.rewardRelation);
        }

        SaveSystem.Save(save);
        RefreshAvailability();
        OnQuestsUpdated?.Invoke();
    }

    public void FailQuest(string questId)
    {
        var save = GameManager.Instance.CurrentSave;
        SetQuestState(questId, QuestState.Available, save);
        EconomyManager.Instance.AddReputation(-5);
        SaveSystem.Save(save);
        OnQuestsUpdated?.Invoke();
    }

    private bool IsUnlocked(QuestData quest, SaveData save)
    {
        var cond = quest.unlockCondition;

        if (!string.IsNullOrEmpty(cond.requiredQuestId))
        {
            var entry = save.questEntries.Find(e => e.questId == cond.requiredQuestId);
            if (entry?.state != QuestState.Completed) return false;
        }

        if (cond.dayThreshold > 0 && save.currentDay < cond.dayThreshold)
            return false;

        if (!string.IsNullOrEmpty(cond.characterId) && cond.relationThreshold > 0)
        {
            var charEntry = save.characterEntries.Find(e => e.characterId == cond.characterId);
            if (charEntry == null || charEntry.relationValue < cond.relationThreshold) return false;
        }

        return true;
    }

    private void SetQuestState(string questId, QuestState state, SaveData save)
    {
        var entry = save.questEntries.Find(e => e.questId == questId);
        if (entry != null)
            entry.state = state;
        else
            save.questEntries.Add(new QuestSaveEntry { questId = questId, state = state });
    }
}
