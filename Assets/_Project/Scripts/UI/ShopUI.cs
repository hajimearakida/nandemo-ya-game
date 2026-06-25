using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private Transform itemContainer;
    [SerializeField] private GameObject itemEntryPrefab;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private Button closeButton;

    void Start()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged += OnStateChanged;
        if (EconomyManager.Instance != null)
            EconomyManager.Instance.OnValuesChanged += Refresh;
        if (closeButton != null)
            closeButton.onClick.AddListener(() => GameManager.Instance.ChangeState(GameState.QuestBoard));
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged -= OnStateChanged;
        if (EconomyManager.Instance != null)
            EconomyManager.Instance.OnValuesChanged -= Refresh;
    }

    private void OnStateChanged(GameState state)
    {
        gameObject.SetActive(state == GameState.Shop);
        if (state == GameState.Shop) Refresh();
    }

    private void Refresh()
    {
        if (goldText != null)
            goldText.text = $"所持金: {EconomyManager.Instance.Gold}G";

        foreach (Transform child in itemContainer)
            Destroy(child.gameObject);

        if (ShopSystem.Instance == null) return;

        foreach (var item in ShopSystem.Instance.GetAllItems())
        {
            var entry = Instantiate(itemEntryPrefab, itemContainer);
            var label = entry.GetComponentInChildren<TextMeshProUGUI>();
            bool purchased = ShopSystem.Instance.IsPurchased(item.itemId);

            if (label != null)
            {
                string status = purchased ? "【購入済み】" : $"{item.cost}G";
                label.text = $"{item.itemName}　{status}\n<size=70%>{item.description}</size>";
            }

            var btn = entry.GetComponent<Button>();
            if (btn != null)
            {
                btn.interactable = !purchased;
                var captured = item.itemId;
                btn.onClick.AddListener(() =>
                {
                    ShopSystem.Instance.TryPurchase(captured);
                    Refresh();
                });
            }
        }
    }
}
