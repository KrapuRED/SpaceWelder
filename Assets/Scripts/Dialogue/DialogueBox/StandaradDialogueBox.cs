using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StandaradDialogueBox : DialogueBox
{
    public Image characterIcon;
    public TextMeshProUGUI characterNameText;
    [SerializeField] private TypeEffect typeEffect;
    public TypeEffect TypeEffect => typeEffect;

    public void SetStandaradDialogueBox(string nameCharacter, string dialogue, Sprite charIcon = null)
    {
        if (charIcon != null)
            characterIcon.sprite = charIcon;

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
