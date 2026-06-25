using System;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance { get; private set; }

    [SerializeField] private CharacterData[] allCharacters;

    public event Action<string, int> OnRelationChanged;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public int GetRelation(string characterId)
    {
        var entry = GameManager.Instance.CurrentSave.characterEntries
            .Find(e => e.characterId == characterId);
        return entry?.relationValue ?? 0;
    }

    public void AddRelation(string characterId, int delta)
    {
        var save = GameManager.Instance.CurrentSave;
        var entry = save.characterEntries.Find(e => e.characterId == characterId);
        if (entry == null)
        {
            entry = new CharacterSaveEntry { characterId = characterId, relationValue = 0 };
            save.characterEntries.Add(entry);
        }

        entry.relationValue = Mathf.Clamp(entry.relationValue + delta, 0, 100);
        OnRelationChanged?.Invoke(characterId, entry.relationValue);
        SaveSystem.Save(save);
        QuestManager.Instance.RefreshAvailability();
    }

    public CharacterData GetData(string characterId)
    {
        return Array.Find(allCharacters, c => c.characterId == characterId);
    }

    public bool AllMainRelationsMaxed()
    {
        string[] mainCharacters = { "belta", "gran", "riri", "dog", "sei", "garo", "pico", "nui_komugi" };
        foreach (var id in mainCharacters)
        {
            if (GetRelation(id) < 80) return false;
        }
        return true;
    }
}
