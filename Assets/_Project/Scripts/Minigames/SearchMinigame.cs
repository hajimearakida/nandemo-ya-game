using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SearchMinigame : BaseMinigame
{
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI hintText;
    [SerializeField] private TextMeshProUGUI remainingText;
    [SerializeField] private Button[] locationButtons;
    [SerializeField] private TextMeshProUGUI[] locationLabels;
    [SerializeField] private SearchData searchData;

    private int _searchCount;
    private const int MaxSearches = 8;

    private static readonly SearchData FallbackData = new SearchData
    {
        description = "依頼の場所を探しています...",
        locations = new[]
        {
            new SearchLocation { locationName = "広場の中心", hint = "ここではないようです", isTarget = false },
            new SearchLocation { locationName = "市場の屋台", hint = "近づいている気がします", isTarget = false },
            new SearchLocation { locationName = "路地裏の奥", hint = "見つけました！", isTarget = true },
            new SearchLocation { locationName = "宿屋の前", hint = "何もありません", isTarget = false },
            new SearchLocation { locationName = "井戸のそば", hint = "人の気配はない", isTarget = false },
            new SearchLocation { locationName = "教会の庭", hint = "静かすぎる", isTarget = false }
        }
    };

    protected override void StartGame()
    {
        _searchCount = 0;
        var data = (searchData != null && searchData.locations != null && searchData.locations.Length > 0)
            ? searchData : FallbackData;

        if (descriptionText != null) descriptionText.text = data.description;
        if (hintText != null) hintText.text = "――　場所を選んで手がかりを探そう";
        UpdateRemaining();
        SetupButtons(data);
    }

    private void SetupButtons(SearchData data)
    {
        for (int i = 0; i < locationButtons.Length; i++)
        {
            if (locationButtons[i] == null) continue;
            bool active = i < data.locations.Length;
            locationButtons[i].gameObject.SetActive(active);
            if (!active) continue;

            if (locationLabels != null && i < locationLabels.Length && locationLabels[i] != null)
                locationLabels[i].text = data.locations[i].locationName;

            int captured = i;
            locationButtons[i].onClick.RemoveAllListeners();
            locationButtons[i].onClick.AddListener(() => Investigate(data.locations[captured]));
        }
    }

    private void Investigate(SearchLocation location)
    {
        _searchCount++;
        if (hintText != null) hintText.text = location.hint;
        UpdateRemaining();

        if (location.isTarget) { Complete(true); return; }
        if (_searchCount >= MaxSearches) Complete(false);
    }

    private void UpdateRemaining()
    {
        if (remainingText != null)
            remainingText.text = $"残り {MaxSearches - _searchCount} 回";
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
