using UnityEngine;

public class PanelDeathPlayer : Panel
{
    private void OnEnable()
    {
        GlobalEvents.OnPlayerDeath.AddListener(ShowPanel);
    }

    private void OnDisable()
    {
        GlobalEvents.OnPlayerDeath.RemoveListener(ShowPanel);
    }

    private void OnDestroy()
    {
        GlobalEvents.OnPlayerDeath.RemoveListener(ShowPanel);

    }

    public override void ShowPanel()
    {
        if (this == null) return;

        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        canvasGroup.alpha = 1f;
    }

    public override void HidePanel()
    {
        if (this == null) return;

        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        canvasGroup.alpha = 0f;
    }
}
