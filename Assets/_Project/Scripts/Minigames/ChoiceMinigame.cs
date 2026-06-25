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

    private static readonly ChoiceQuestion[] FallbackQuestions = new[]
    {
        new ChoiceQuestion
        {
            questionText = "依頼の内容を正確に把握しましたか？",
            choices = new[]
            {
                new ChoiceOption { label = "はい、把握しています", isCorrect = true },
                new ChoiceOption { label = "なんとなくしか…", isCorrect = false }
            }
        },
        new ChoiceQuestion
        {
            questionText = "依頼人が最も求めているものは？",
            choices = new[]
            {
                new ChoiceOption { label = "誠実な対応", isCorrect = true },
                new ChoiceOption { label = "とにかく早さ", isCorrect = false },
                new ChoiceOption { label = "安い料金", isCorrect = false }
            }
        },
        new ChoiceQuestion
        {
            questionText = "困ったときは？",
            choices = new[]
            {
                new ChoiceOption { label = "正直に状況を伝える", isCorrect = true },
                new ChoiceOption { label = "何も言わず続ける", isCorrect = false }
            }
        }
    };

    protected override void StartGame()
    {
        _questionIndex = 0;
        _correctCount = 0;
        if (questions == null || questions.Length == 0) questions = FallbackQuestions;
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
        if (questionText != null) questionText.text = q.questionText;

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (choiceButtons[i] == null) continue;
            bool active = i < q.choices.Length;
            choiceButtons[i].gameObject.SetActive(active);
            if (!active) continue;

            if (choiceLabels[i] != null) choiceLabels[i].text = q.choices[i].label;
            int captured = i;
            choiceButtons[i].onClick.RemoveAllListeners();
            choiceButtons[i].onClick.AddListener(() => OnSelect(captured));
        }
    }

    private void OnSelect(int index)
    {
        if (questions[_questionIndex].choices[index].isCorrect) _correctCount++;
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
