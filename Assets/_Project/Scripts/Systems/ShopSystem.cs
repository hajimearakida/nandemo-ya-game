using System;
using UnityEngine;

public class ShopSystem : MonoBehaviour
{
    public static ShopSystem Instance { get; private set; }

    [SerializeField] private ShopItemData[] allItems;

    public event Action OnPurchased;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public ShopItemData[] GetAllItems() => allItems;

    public bool IsPurchased(string itemId)
    {
        return GameManager.Instance.CurrentSave.purchasedShopItemIds.Contains(itemId);
    }

    public bool TryPurchase(string itemId)
    {
        var item = Array.Find(allItems, i => i.itemId == itemId);
        if (item == null || IsPurchased(itemId)) return false;
        if (!EconomyManager.Instance.SpendGold(item.cost)) return false;

        GameManager.Instance.CurrentSave.purchasedShopItemIds.Add(itemId);
        SaveSystem.Save(GameManager.Instance.CurrentSave);
        OnPurchased?.Invoke();
        return true;
    }

    public float GetEffectValue(ShopEffectType effectType)
    {
        float total = 0f;
        foreach (var item in allItems)
        {
            if (item.effectType == effectType && IsPurchased(item.itemId))
                total += item.effectValue;
        }
        return total;
    }
}
