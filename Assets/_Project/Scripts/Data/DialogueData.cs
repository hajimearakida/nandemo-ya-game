using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "NandemoYa/Dialogue")]
public class DialogueData : ScriptableObject
{
    public string dialogueId;
    public DialogueLine[] lines;
}

[Serializable]
public class DialogueLine
{
    public string characterId;
    [TextArea(2, 5)]
    public string text;
    public DialogueChoice[] choices;

    public bool HasChoices => choices != null && choices.Length > 0;
}

[Serializable]
public class DialogueChoice
{
    public string label;
    public string nextDialogueId;
    public string resultFlag;
    public int reputationDelta;
    public int relationDelta;
}
