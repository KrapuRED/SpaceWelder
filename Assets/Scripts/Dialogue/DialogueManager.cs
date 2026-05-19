using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public enum CharacterType
{
    None,
    Captian,
    Standard
}

[System.Serializable]
public class StoryDialogueData
{
    public string characterName;
    public CharacterType charType;
    public DialogueData dialogueData;
}

[System.Serializable]
public class StoryData
{
    public string clientName;
    public string cargo;
    public List<StoryDialogueData> storyDialogueDatas = new();

}

public class DialogueManager : MonoBehaviour
{
    public DialogueBox captianDialogueBox;
    public DialogueBox standardDialogueBox;

    [SerializeField] private StoryData storyData;

    private int _dialogueCount;

    public void ContinueDialogue()
    {
        if (captianDialogueBox == null || standardDialogueBox == null) return;

        captianDialogueBox.HideDialogueBox();
        standardDialogueBox.HideDialogueBox();

        if (_dialogueCount >= storyData.storyDialogueDatas.Count)
        {
            OnEndStory();
            return;
        }

        var nextDialogue = storyData.storyDialogueDatas[_dialogueCount];

        if (nextDialogue.charType == CharacterType.Captian)
        {
            captianDialogueBox.ShowDialogueBox();
        }
        else
        {
            standardDialogueBox.ShowDialogueBox();
        }
    }

    public void OnEndStory()
    {
        //Change Scene to Main-GamePlay-Levelx via GameManager
        Debug.Log("Story is Done! Please Change Scene to Main-GamePlay-Level");
    }

}
