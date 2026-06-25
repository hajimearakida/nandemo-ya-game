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

    protected override void StartGame()
    {
        _stepIndex = 0;
        ShowStep();
    }

    private void ShowStep()
    {
        if (_stepIndex >= eventData.steps.Length)
        {
            Complete(true);
            return;
        }

        var step = eventData.steps[_stepIndex];
        narrativeText.text = step.narrative;

        for (int i = 0; i < actionButtons.Length; i++)
        {
            bool active = i < step.actions.Length;
            actionButtons[i].gameObject.SetActive(active);
            if (!active) continue;

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
