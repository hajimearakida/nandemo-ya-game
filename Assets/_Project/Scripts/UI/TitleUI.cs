using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TitleUI : MonoBehaviour
{
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI subtitleText;

    void Start()
    {
        if (titleText != null)    titleText.text = "なんでも屋";
        if (subtitleText != null) subtitleText.text = "〜旅の便利屋、はじめます〜";

        if (newGameButton != null)
            newGameButton.onClick.AddListener(StartNewGame);

        bool hasSave = SaveSystem.HasSaveData();
        if (continueButton != null)
        {
            continueButton.interactable = hasSave;
            continueButton.onClick.AddListener(ContinueGame);
        }
    }

    private void StartNewGame()
    {
        SaveSystem.Delete();
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
    }

    private void ContinueGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
    }
}
