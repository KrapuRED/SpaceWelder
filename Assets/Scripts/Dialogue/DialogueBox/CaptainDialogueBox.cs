using UnityEngine;
using TMPro;

public class CaptainDialogueBox : DialogueBox
{
    public TextMeshProUGUI characterNameText;
    [SerializeField] private TypeEffect typeEffect;
    public TypeEffect TypeEffect => typeEffect;

    public void SetCaptainDialogueBox(string nameCharacter, string dialogue)
    {
        characterNameText.text = nameCharacter;
        SetDialogueText(dialogue);
    }

    public override void ShowDialogueBox()
    {
        if (this == null) return;

        canvasGroup.alpha = 1;
    }

    public override void HideDialogueBox()
    {
        if (this == null) return;

        canvasGroup.alpha = 0;
    }

    public override void SetDialogueText(string dialogueTextContex)
    {
        typeEffect.PlayText(dialogueTextContex);
        ShowDialogueBox();
    }
}
