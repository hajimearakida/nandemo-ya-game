using System;
using UnityEngine;

public enum MinigameType
{
    Choice,
    Search,
    Puzzle,
    Timing,
    Special
}

[CreateAssetMenu(fileName = "NewQuest", menuName = "NandemoYa/Quest")]
public class QuestData : ScriptableObject
{
    public string questId;
    public string questName;
    [TextArea(2, 4)]
    public string description;
    public string characterId;
    public MinigameType minigameType;
    [Range(1, 3)]
    public int difficulty = 1;
    public int rewardGold;
    public int rewardReputation;
    public int rewardRelation;
    public string dialogueId;
    public UnlockCondition unlockCondition;
}

[Serializable]
public class UnlockCondition
{
    public string requiredQuestId;
    public string characterId;
    public int relationThreshold;
    public int dayThreshold;

    public bool HasConditions =>
        !string.IsNullOrEmpty(requiredQuestId) ||
        dayThreshold > 0 ||
        (!string.IsNullOrEmpty(characterId) && relationThreshold > 0);
}
