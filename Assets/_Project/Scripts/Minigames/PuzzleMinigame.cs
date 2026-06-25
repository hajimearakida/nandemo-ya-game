using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PuzzleMinigame : BaseMinigame
{
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private TextMeshProUGUI selectedOrderText;
    [SerializeField] private Button[] itemButtons;
    [SerializeField] private TextMeshProUGUI[] itemLabels;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private PuzzleData puzzleData;

    private List<string> _selected = new();
    private string[] _shuffled;

    private static readonly PuzzleData FallbackData = new PuzzleData
    {
        instruction = "作業を正しい順番に並べてください",
        correctOrder = new[] { "準備する", "道具を選ぶ", "作業する", "確認する" }
    };

    protected override void StartGame()
    {
        _selected.Clear();
        var data = (puzzleData != null && puzzleData.correctOrder != null && puzzleData.correctOrder.Length > 0)
            ? puzzleData : FallbackData;

        if (instructionText != null) instructionText.text = data.instruction;

        _shuffled = data.correctOrder.OrderBy(_ => UnityEngine.Random.value).ToArray();

        if (confirmButton != null)
        {
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(() => CheckAnswer(data));
        }
        if (resetButton != null)
        {
            resetButton.onClick.RemoveAllListeners();
            resetButton.onClick.AddListener(ResetSelection);
        }

        SetupButtons();
        UpdateDisplay();
    }

    private void SetupButtons()
    {
        for (int i = 0; i < itemButtons.Length; i++)
        {
            if (itemButtons[i] == null) continue;
            bool active = i < _shuffled.Length;
            itemButtons[i].gameObject.SetActive(active);
            if (!active) continue;

            if (itemLabels != null && i < itemLabels.Length && itemLabels[i] != null)
                itemLabels[i].text = _shuffled[i];

            var img = itemButtons[i].GetComponent<Image>();
            if (img != null) img.color = new Color(0.2f, 0.35f, 0.55f);

            string captured = _shuffled[i];
            int idx = i;
            itemButtons[i].onClick.RemoveAllListeners();
            itemButtons[i].onClick.AddListener(() => ToggleItem(captured, idx));
        }
    }

    private void ToggleItem(string item, int btnIndex)
    {
        if (_selected.Contains(item))
        {
            _selected.Remove(item);
            if (itemButtons[btnIndex] != null)
            {
                var img = itemButtons[btnIndex].GetComponent<Image>();
                if (img != null) img.color = new Color(0.2f, 0.35f, 0.55f);
            }
        }
        else
        {
            _selected.Add(item);
            if (itemButtons[btnIndex] != null)
            {
                var img = itemButtons[btnIndex].GetComponent<Image>();
                if (img != null) img.color = new Color(0.8f, 0.75f, 0.2f);
            }
        }
        UpdateDisplay();
    }

    private void ResetSelection()
    {
        _selected.Clear();
        for (int i = 0; i < itemButtons.Length; i++)
        {
            if (itemButtons[i] == null) continue;
            var img = itemButtons[i].GetComponent<Image>();
            if (img != null) img.color = new Color(0.2f, 0.35f, 0.55f);
        }
        UpdateDisplay();
    }

    private void CheckAnswer(PuzzleData data)
    {
        Complete(_selected.SequenceEqual(data.correctOrder));
    }

    private void UpdateDisplay()
    {
        if (selectedOrderText != null)
            selectedOrderText.text = _selected.Count > 0
                ? string.Join(" → ", _selected)
                : "順番に選んでください";
    }
}

[Serializable]
public class PuzzleData
{
    [TextArea(1, 3)] public string instruction;
    public string[] correctOrder;
}
