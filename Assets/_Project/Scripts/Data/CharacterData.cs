using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacter", menuName = "NandemoYa/Character")]
public class CharacterData : ScriptableObject
{
    public string characterId;
    public string characterName;
    public Sprite icon;
    [TextArea(2, 4)]
    public string introductionText;
}
