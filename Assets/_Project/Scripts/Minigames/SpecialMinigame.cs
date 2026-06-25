using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpecialMinigame : BaseMinigame
{
    [SerializeField] private TextMeshProUGUI narrativeText;
    [SerializeField] private Button[] actionButtons;
    [SerializeField] private TextMeshProUGUI[] actionLabels;
    [SerializeField] private SpecialEventData eventData;

    private int _stepIndex;

    private static readonly SpecialEventData FallbackData = new SpecialEventData
    {
        steps = new[]
        {
            new SpecialStep
            {
                narrative = "不思議な出来事が起きています…\n静かに状況を観察した。\nどう対応しますか？",
                actions = new[]
                {
                    new SpecialAction { label = "慎重に、でも誠実に", isEnding = true, isSuccess = true },
                    new SpecialAction { label = "大胆に行動する", isEnding = false, isSuccess = false }
                }
            },
            new SpecialStep
            {
                narrative = "行動したことで状況が動いた。\n次はどうする？",
                actions = new[]
                {
                    new SpecialAction { label = "落ち着いて判断する", isEnding = true, isSuccess = true },
                    new SpecialAction { label = "そのまま突き進む", isEnding = true, isSuccess = false }
                }
            }
        }
    };

    protected override void StartGame()
    {
        _stepIndex = 0;
        if (eventData == null || eventData.steps == null || eventData.steps.Length == 0)
            eventData = FallbackData;
        ShowStep();
    }

    private void ShowStep()
    {
        if (_stepIndex >= eventData.steps.Length) { Complete(true); return; }

        var step = eventData.steps[_stepIndex];
        if (narrativeText != null) narrativeText.text = step.narrative;

        for (int i = 0; i < actionButtons.Length; i++)
        {
            if (actionButtons[i] == null) continue;
            bool active = i < step.actions.Length;
            actionButtons[i].gameObject.SetActive(active);
            if (!active) continue;

            if (actionLabels != null && i < actionLabels.Length && actionLabels[i] != null)
                actionLabels[i].text = step.actions[i].label;

            int captured = i;
            actionButtons[i].onClick.RemoveAllListeners();
            actionButtons[i].onClick.AddListener(() => OnAction(captured));
        }
    }

    private void OnAction(int index)
    {
        var action = eventData.steps[_stepIndex].actions[index];
        if (action.isEnding) { Complete(action.isSuccess); return; }
        _stepIndex++;
        ShowStep();
    }
}

[Serializable]
public class SpecialEventData
{
    public SpecialStep[] steps;
}

[Serializable]
public class SpecialStep
{
    [TextArea(2, 5)] public string narrative;
    public SpecialAction[] actions;
}

[Serializable]
public class SpecialAction
{
    public string label;
    public bool isEnding;
    public bool isSuccess;
}
