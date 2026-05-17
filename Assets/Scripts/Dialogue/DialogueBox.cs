using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueBox : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI dialogueText;
    public float visiableTime;

    private Coroutine _visibelTimeCoroutine;

    private void OnEnable()
    {
        GlobalEvents.OnMissionControlDialogue.AddListener(SetDialogueText);
    }

    private void OnDisable()
    {
        GlobalEvents.OnMissionControlDialogue.RemoveListener(SetDialogueText);
    }


    public void ShowDialogueBox()
    {
        if (canvasGroup == null) return;

        canvasGroup.alpha = 1;

        if (_visibelTimeCoroutine != null) StopCoroutine(_visibelTimeCoroutine);
        _visibelTimeCoroutine = StartCoroutine(VisibelTime());
    }

    public void HideDialogueBox()
    {
        canvasGroup.alpha = 0;
    }

    public void SetDialogueText(string dialogueTextContex)
    {
        ShowDialogueBox();
        dialogueText.text = dialogueTextContex;
    }

    IEnumerator VisibelTime()
    {
        yield return new WaitForSeconds(visiableTime);
        HideDialogueBox();
    }
}
