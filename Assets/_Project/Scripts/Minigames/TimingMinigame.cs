using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimingMinigame : BaseMinigame
{
    [SerializeField] private Slider gauge;
    [SerializeField] private RectTransform successZoneIndicator;
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Button tapButton;

    [Header("設定")]
    [SerializeField] private float gaugeSpeed = 0.7f;
    [SerializeField] private float successZoneMin = 0.35f;
    [SerializeField] private float successZoneMax = 0.65f;
    [SerializeField] private int requiredSuccesses = 3;
    [SerializeField] private int totalAttempts = 5;

    private float _gaugeValue;
    private bool _ascending = true;
    private bool _running;
    private int _successCount;
    private int _attemptCount;

    protected override void StartGame()
    {
        _gaugeValue = 0f;
        _ascending = true;
        _running = true;
        _successCount = 0;
        _attemptCount = 0;

        if (gauge != null) gauge.value = 0f;
        if (resultText != null) resultText.text = "";
        if (tapButton != null)
        {
            tapButton.onClick.RemoveAllListeners();
            tapButton.onClick.AddListener(OnTap);
        }
        UpdateInstruction();
        PositionSuccessZone();
    }

    void Update()
    {
        if (!_running) return;

        _gaugeValue += (_ascending ? 1f : -1f) * gaugeSpeed * Time.deltaTime;
        if (_gaugeValue >= 1f) { _gaugeValue = 1f; _ascending = false; }
        if (_gaugeValue <= 0f) { _gaugeValue = 0f; _ascending = true; }
        if (gauge != null) gauge.value = _gaugeValue;
    }

    private void OnTap()
    {
        if (!_running) return;

        bool hit = _gaugeValue >= successZoneMin && _gaugeValue <= successZoneMax;
        if (hit) _successCount++;
        _attemptCount++;

        if (resultText != null) resultText.text = hit ? "いい感じ！" : "ズレた…";
        UpdateInstruction();

        if (_attemptCount >= totalAttempts)
        {
            _running = false;
            Complete(_successCount >= requiredSuccesses);
        }
    }

    private void UpdateInstruction()
    {
        if (instructionText != null)
            instructionText.text = $"ゲージが光っている間にタップ！\n成功: {_successCount} / {requiredSuccesses}　残り: {totalAttempts - _attemptCount}回";
    }

    private void PositionSuccessZone()
    {
        if (successZoneIndicator == null || gauge == null) return;
        var gaugeRect = gauge.GetComponent<RectTransform>();
        if (gaugeRect == null) return;
        float gaugeWidth = gaugeRect.rect.width;
        float centerX = (successZoneMin + successZoneMax) * 0.5f * gaugeWidth - gaugeWidth * 0.5f;
        float zoneWidth = (successZoneMax - successZoneMin) * gaugeWidth;
        successZoneIndicator.anchoredPosition = new Vector2(centerX, 0f);
        successZoneIndicator.sizeDelta = new Vector2(zoneWidth, successZoneIndicator.sizeDelta.y);
    }
}
