using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChoiceMinigame : BaseMinigame
{
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private Button[] choiceButtons;
    [SerializeField] private TextMeshProUGUI[] choiceLabels;
    [SerializeField] private ChoiceQuestion[] questions;

    private int _questionIndex;
    private int _correctCount;

    protected override void StartGame()
    {
        _questionIndex = 0;
        _correctCount = 0;
        ShowQuestion();
    }

    private void ShowQuestion()
    {
        if (_questionIndex >= questions.Length)
        {
            int required = Mathf.CeilToInt(questions.Length * 0.5f);
            Complete(_correctCount >= required);
            return;
        }

        var q = questions[_questionIndex];
        questionText.text = q.questionText;

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            bool active = i < q.choices.Length;
            choiceButtons[i].gameObject.SetActive(active);
            if (!active) continue;

            choiceLabels[i].text = q.choices[i].label;
            int captured = i;
            choiceButtons[i].onClick.RemoveAllListeners();
            choiceButtons[i].onClick.AddListener(() => OnSelect(captured));
        }
    }

    private void OnSelect(int index)
    {
        if (questions[_questionIndex].choices[index].isCorrect)
            _correctCount++;
        _questionIndex++;
        ShowQuestion();
    }
}

[Serializable]
public class ChoiceQuestion
{
    [TextArea(2, 3)] public string questionText;
    public ChoiceOption[] choices;
}

[Serializable]
public class ChoiceOption
{
    public string label;
    public bool isCorrect;
}
