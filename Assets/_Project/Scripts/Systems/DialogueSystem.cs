using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem Instance { get; private set; }

    public event Action<DialogueLine> OnLineDisplayed;
    public event Action<DialogueChoice[]> OnChoicesDisplayed;
    public event Action<string> OnDialogueCompleted;

    private DialogueData _current;
    private int _lineIndex;
    private Dictionary<string, DialogueData> _cache = new();

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void StartDialogue(DialogueData data)
    {
        _current = data;
        _lineIndex = 0;
        ShowLine();
    }

    public void Next()
    {
        if (_current == null) return;

        var line = _current.lines[_lineIndex];
        if (line.HasChoices) return;

        _lineIndex++;
        if (_lineIndex >= _current.lines.Length)
        {
            Complete(string.Empty);
            return;
        }
        ShowLine();
    }

    public void SelectChoice(int index)
    {
        if (_current == null) return;

        var choice = _current.lines[_lineIndex].choices[index];
        ApplyChoiceEffects(choice);

        if (!string.IsNullOrEmpty(choice.nextDialogueId) && _cache.TryGetValue(choice.nextDialogueId, out var next))
        {
            StartDialogue(next);
            return;
        }

        Complete(choice.resultFlag);
    }

    public void RegisterDialogue(DialogueData data)
    {
        _cache[data.dialogueId] = data;
    }

    private void ShowLine()
    {
        var line = _current.lines[_lineIndex];
        OnLineDisplayed?.Invoke(line);
        if (line.HasChoices)
            OnChoicesDisplayed?.Invoke(line.choices);
    }

    private void ApplyChoiceEffects(DialogueChoice choice)
    {
        if (GameManager.Instance?.CurrentSave == null) return;

        GameManager.Instance.CurrentSave.reputation =
            Mathf.Clamp(GameManager.Instance.CurrentSave.reputation + choice.reputationDelta, 0, 100);
    }

    private void Complete(string resultFlag)
    {
        _current = null;
        GameManager.Instance?.ChangeState(GameState.Result);
        OnDialogueCompleted?.Invoke(resultFlag);
    }
}
