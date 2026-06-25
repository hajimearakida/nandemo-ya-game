using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SearchMinigame : BaseMinigame
{
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI hintText;
    [SerializeField] private TextMeshProUGUI remainingText;
    [SerializeField] private Transform locationContainer;
    [SerializeField] private GameObject locationButtonPrefab;
    [SerializeField] private SearchData searchData;

    private int _searchCount;
    private int _maxSearches = 8;

    protected override void StartGame()
    {
        _searchCount = 0;
        descriptionText.text = searchData.description;
        hintText.text = "";
        UpdateRemaining();
        BuildButtons();
    }

    private void BuildButtons()
    {
        foreach (Transform child in locationContainer)
            Destroy(child.gameObject);

        foreach (var loc in searchData.locations)
        {
            var btn = Instantiate(locationButtonPrefab, locationContainer);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = loc.locationName;
            var captured = loc;
            btn.GetComponent<Button>().onClick.AddListener(() => Investigate(captured));
        }
    }

    private void Investigate(SearchLocation location)
    {
        _searchCount++;
        hintText.text = location.hint;
        UpdateRemaining();

        if (location.isTarget) { Complete(true); return; }
        if (_searchCount >= _maxSearches) Complete(false);
    }

    private void UpdateRemaining()
    {
        remainingText.text = $"残り {_maxSearches - _searchCount} 回";
    }
}

[Serializable]
public class SearchData
{
    [TextArea(2, 3)] public string description;
    public SearchLocation[] locations;
}

[Serializable]
public class SearchLocation
{
    public string locationName;
    [TextArea(1, 3)] public string hint;
    public bool isTarget;
}
