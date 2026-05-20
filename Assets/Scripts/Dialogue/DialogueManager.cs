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
    public CaptainDialogueBox captainDialogueBox;
    public StandaradDialogueBox standardDialogueBox;

    [SerializeField] private StoryData storyData;
    [SerializeField] private PlayerInput playerInput;

    private int _dialogueCount;

    private void Start()
    {
        TriggerDialogue(storyData.storyDialogueDatas[_dialogueCount]);

        if (storyLogUI != null)
            storyLogUI.SetStoryLogUI(storyData.cargo, storyData.clientName);
    }

    public void OnPressContinue(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        bool isTyping = captainDialogueBox.TypeEffect.IsTyping
                     || standardDialogueBox.TypeEffect.IsTyping;

        if (isTyping)
        {
            captainDialogueBox.TypeEffect.Skip();
            standardDialogueBox.TypeEffect.Skip();
            return;
        }

        if (_dialogueCount < storyData.storyDialogueDatas.Count)
            ContinueDialogue();
    }

    public void ContinueDialogue()
    {

        if (captainDialogueBox == null || standardDialogueBox == null) return;

        captainDialogueBox.HideDialogueBox();
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
        if (storyDialogue.charType == CharacterType.Captian)
        {
           captainDialogueBox.SetCaptainDialogueBox(storyDialogue.characterName, storyDialogue.dialogueData.dialogue);
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
        GameManager.Instance.NextLevel();

    }

}
