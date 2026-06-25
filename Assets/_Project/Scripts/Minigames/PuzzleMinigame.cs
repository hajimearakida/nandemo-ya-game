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
    [SerializeField] private Transform itemContainer;
    [SerializeField] private GameObject itemButtonPrefab;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private PuzzleData puzzleData;

    private List<string> _selected = new();
    private Dictionary<string, Button> _itemButtons = new();

    protected override void StartGame()
    {
        _selected.Clear();
        _itemButtons.Clear();
        instructionText.text = puzzleData.instruction;
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(CheckAnswer);
        resetButton.onClick.RemoveAllListeners();
        resetButton.onClick.AddListener(ResetSelection);
        BuildItems();
        UpdateDisplay();
    }

    private void BuildItems()
    {
        foreach (Transform child in itemContainer)
            Destroy(child.gameObject);

        var shuffled = puzzleData.correctOrder.OrderBy(_ => UnityEngine.Random.value).ToArray();
        foreach (var item in shuffled)
        {
            var btn = Instantiate(itemButtonPrefab, itemContainer);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = item;
            var captured = item;
            var btnComp = btn.GetComponent<Button>();
            btnComp.onClick.AddListener(() => ToggleItem(captured));
            _itemButtons[item] = btnComp;
        }
    }

    private void ToggleItem(string item)
    {
        if (_selected.Contains(item))
        {
            _selected.Remove(item);
            _itemButtons[item].GetComponent<Image>().color = Color.white;
        }
        else
        {
            _selected.Add(item);
            _itemButtons[item].GetComponent<Image>().color = new Color(1f, 0.9f, 0.4f);
        }
        UpdateDisplay();
    }

    private void ResetSelection()
    {
        _selected.Clear();
        foreach (var btn in _itemButtons.Values)
            btn.GetComponent<Image>().color = Color.white;
        UpdateDisplay();
    }

    private void CheckAnswer()
    {
        Complete(_selected.SequenceEqual(puzzleData.correctOrder));
    }

    private void UpdateDisplay()
    {
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
