using UnityEngine;
using UnityEngine.InputSystem;
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
    public Sprite characterIcon;
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
    public StoryLogUI storyLogUI;
    public CaptainDialogueBox captianDialogueBox;
    public StandaradDialogueBox standardDialogueBox;

    [SerializeField] private StoryData storyData;

    private int _dialogueCount;

    private void Start()
    {
        TriggerDialogue(storyData.storyDialogueDatas[_dialogueCount]);
        storyLogUI.SetStoryLogUI(storyData.cargo, storyData.clientName);
    }

    public void OnPressContinue(InputAction.CallbackContext context)
    {
        if (context.started && _dialogueCount < storyData.storyDialogueDatas.Count)
            ContinueDialogue();
    }

    public void ContinueDialogue()
    {

        if (captianDialogueBox == null || standardDialogueBox == null) return;

        captianDialogueBox.HideDialogueBox();
        standardDialogueBox.HideDialogueBox();

        _dialogueCount++;

        if (_dialogueCount >= storyData.storyDialogueDatas.Count)
        {
            OnEndStory();
            return;
        }

        var nextDialogue = storyData.storyDialogueDatas[_dialogueCount];

        TriggerDialogue(nextDialogue);
    }

    private void TriggerDialogue(StoryDialogueData storyDialogue)
    {
        Debug.Log($"[{storyDialogue.characterName}] : {storyDialogue.dialogueData.dialogue}");

        if (storyDialogue.charType == CharacterType.Captian)
        {
           captianDialogueBox.SetCaptainDialogueBox(storyDialogue.characterName, storyDialogue.dialogueData.dialogue);
        }
        else
        {
            standardDialogueBox.SetStandaradDialogueBox(storyDialogue.characterName, storyDialogue.dialogueData.dialogue, storyDialogue.characterIcon);
        }
    }

    public void OnEndStory()
    {
        //Change Scene to Main-GamePlay-Levelx via GameManager
        Debug.Log("Story is Done! Please Change Scene to Main-GamePlay-Level");
    }

}
