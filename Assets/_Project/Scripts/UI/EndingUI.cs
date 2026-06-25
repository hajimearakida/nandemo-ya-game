using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndingUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI endingTitleText;
    [SerializeField] private TextMeshProUGUI endingBodyText;
    [SerializeField] private Button titleReturnButton;

    void Start()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged += OnStateChanged;
        titleReturnButton.onClick.AddListener(() => GameManager.Instance.LoadScene("TitleScene"));
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged -= OnStateChanged;
    }

    private void OnStateChanged(GameState state)
    {
        if (state != GameState.Ending && state != GameState.GameOver) return;

        gameObject.SetActive(true);

        if (state == GameState.GameOver)
        {
            ShowEnding(EndingType.Bankrupt);
            return;
        }

        var endingType = EndingManager.Instance.DetermineEnding();
        ShowEnding(endingType);
        AchievementManager.Instance.CheckAutoAchievements();
    }

    private void ShowEnding(EndingType type)
    {
        switch (type)
        {
            case EndingType.HereIsHome:
                endingTitleText.text = "「ここが家」エンド";
                endingBodyText.text = "旅をやめることにした。\nこの町が、俺の居場所になっていた。";
                break;
            case EndingType.BackToTravel:
                endingTitleText.text = "「また旅へ」エンド";
                endingBodyText.text = "60日が終わり、また旅に出ることにした。\n……でも、いつかまた戻ってくる気がした。";
                break;
            case EndingType.Bankrupt:
                endingTitleText.text = "「廃業」エンド";
                endingBodyText.text = "なんでも屋は静かに幕を閉じた。\n次の町では、うまくやれるといいが。";
                break;
            case EndingType.MysteriousClient:
                endingTitleText.text = "「謎の依頼人」エンド";
                endingBodyText.text = "タマとロッカが教えてくれた、この町の本当の秘密。\nそれを知っても、俺はここにいたいと思った。";
                break;
            case EndingType.PerfectHandyman:
                endingTitleText.text = "「完璧な便利屋」エンド";
                endingBodyText.text = "50件の依頼、すべてこなした。\nこの町に、俺の手が届かない場所はない。";
                break;
        }
    }
}
