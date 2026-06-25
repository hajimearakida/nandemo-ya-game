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
        goldText.text = $"所持金: {EconomyManager.Instance.Gold}G";

        foreach (Transform child in itemContainer)
            Destroy(child.gameObject);

        foreach (var item in ShopSystem.Instance.GetAllItems())
        {
            var entry = Instantiate(itemEntryPrefab, itemContainer);
            var texts = entry.GetComponentsInChildren<TextMeshProUGUI>();
            texts[0].text = item.itemName;
            texts[1].text = item.description;
            texts[2].text = $"{item.cost}G";

            var btn = entry.GetComponentInChildren<Button>();
            bool purchased = ShopSystem.Instance.IsPurchased(item.itemId);
            btn.interactable = !purchased;
            btn.GetComponentInChildren<TextMeshProUGUI>().text = purchased ? "購入済み" : "購入";

            var captured = item.itemId;
            btn.onClick.AddListener(() =>
            {
                ShopSystem.Instance.TryPurchase(captured);
                Refresh();
            });
        }
    }
}
