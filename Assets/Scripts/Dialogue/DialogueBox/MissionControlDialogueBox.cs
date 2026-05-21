using System.Collections;
using TMPro;
using UnityEngine;

public class MissionControlDialogueBox : MonoBehaviour
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

    private void OnDestroy()
    {
        GlobalEvents.OnMissionControlDialogue.RemoveListener(SetDialogueText);
    }

    public void ShowDialogueBox()
    {
        if (gameObject == null) return;

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
        Debug.Log("MissionControlDialogueBox SetDialogueText: " + dialogueTextContex);
        ShowDialogueBox();
        dialogueText.text = dialogueTextContex;
    }

    IEnumerator VisibelTime()
    {
        yield return new WaitForSeconds(visiableTime);
        HideDialogueBox();
    }
}
