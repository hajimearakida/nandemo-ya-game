using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public int currentDay = 1;
    public int gold = 500;
    public int reputation = 50;

    public List<QuestSaveEntry> questEntries = new();
    public List<CharacterSaveEntry> characterEntries = new();
    public List<string> purchasedShopItemIds = new();
    public List<string> unlockedAchievementIds = new();
}

[Serializable]
public class QuestSaveEntry
{
    public string questId;
    public QuestState state;
}

[Serializable]
public class CharacterSaveEntry
{
    public string characterId;
    public int relationValue;
}

public enum QuestState
{
    Locked,
    Available,
    Active,
    Completed,
    Failed
}
