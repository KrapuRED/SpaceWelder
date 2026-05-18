using UnityEngine;

public class PanelUpgrade : Panel
{
    private void OnEnable()
    {
        GlobalEvents.OnShowPanel.AddListener(ShowPanel);
    }

    private void OnDisable()
    {
        GlobalEvents.OnShowPanel.RemoveListener(ShowPanel);

    }

    public override void ShowPanel()
    {
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
    }

    public override void HidePanel()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;

    }
}
