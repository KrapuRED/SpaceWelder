
using UnityEngine;

[System.Serializable]
public class DialogueData
{
    public string dialogueName;
    public string dialogueID;
    [TextArea(5, 10)]
    public string dialogue;
}
