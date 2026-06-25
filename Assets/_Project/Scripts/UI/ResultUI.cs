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
            GameManager.Instance.OnStateChanged += OnStateChanged;
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
    }

    public void ShowResult(string questName, bool success, int goldReward, int repReward)
    {
        resultText.text = success ? $"「{questName}」\n依頼完了！" : $"「{questName}」\n依頼失敗…";
        rewardText.text = success ? $"+{goldReward}G  評判 +{repReward}" : "評判 -5";
    }
}
