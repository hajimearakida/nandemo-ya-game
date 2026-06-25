using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI rewardText;
    [SerializeField] private Button continueButton;

    void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStateChanged += OnStateChanged;
            OnStateChanged(GameManager.Instance.CurrentState);
        }
        if (continueButton != null)
            continueButton.onClick.AddListener(() => GameManager.Instance.ChangeState(GameState.QuestBoard));
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged -= OnStateChanged;
    }

    private void OnStateChanged(GameState state)
    {
        gameObject.SetActive(state == GameState.Result);
        if (state != GameState.Result) return;

        var mm = MinigameManager.Instance;
        if (mm == null) return;

        if (mm.LastSuccess)
        {
            resultText.text = $"「{mm.LastQuestName}」\n依頼完了！";
            rewardText.text = $"+{mm.LastGoldReward}G　評判 +{mm.LastRepReward}";
        }
        else
        {
            resultText.text = $"「{mm.LastQuestName}」\n依頼失敗…";
            rewardText.text = "評判 -5";
        }
    }
}
