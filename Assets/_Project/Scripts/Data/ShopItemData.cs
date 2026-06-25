using UnityEngine;

public enum ShopEffectType
{
    ExtendPuzzleTime,
    AddSearchHints,
    AddChoiceOption,
    LooseTimingJudge
}

[CreateAssetMenu(fileName = "NewShopItem", menuName = "NandemoYa/ShopItem")]
public class ShopItemData : ScriptableObject
{
    public string itemId;
    public string itemName;
    [TextArea(1, 3)]
    public string description;
    public int cost;
    public ShopEffectType effectType;
    public float effectValue;
}
