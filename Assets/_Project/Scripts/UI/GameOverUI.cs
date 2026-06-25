using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button titleButton;

    void Start()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged += OnStateChanged;
        if (retryButton != null)
            retryButton.onClick.AddListener(() => GameManager.Instance.StartNewGame());
        if (titleButton != null)
            titleButton.onClick.AddListener(() => GameManager.Instance.LoadScene("TitleScene"));
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged -= OnStateChanged;
    }

    private void OnStateChanged(GameState state)
    {
        gameObject.SetActive(state == GameState.GameOver);
        if (state != GameState.GameOver) return;

        if (messageText != null)
            messageText.text = "評判が地に落ちた……\nなんでも屋は廃業となった。";
    }
}
