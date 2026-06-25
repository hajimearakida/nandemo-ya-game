using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DayStartUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI reputationText;
    [SerializeField] private Button startButton;

    void Start()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged += OnStateChanged;
        startButton.onClick.AddListener(() => GameManager.Instance.ChangeState(GameState.QuestBoard));
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged -= OnStateChanged;
    }

    private void OnStateChanged(GameState state)
    {
        gameObject.SetActive(state == GameState.DayStart);
        if (state != GameState.DayStart) return;

        var save = GameManager.Instance.CurrentSave;
        dayText.text = $"{save.currentDay}日目";
        goldText.text = $"所持金: {save.gold}G";
        reputationText.text = $"評判: {save.reputation}";

        QuestManager.Instance.RefreshAvailability();
    }
}
