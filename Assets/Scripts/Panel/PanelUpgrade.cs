using UnityEngine;

public class PanelUpgrade : Panel
{
    private void OnEnable()
    {
        GlobalEvents.OnShowUpgradePanel.AddListener(ShowPanel);
        GlobalEvents.OnHideUpgradePanel.AddListener(HidePanel);
    }

    private void OnDisable()
    {
        GlobalEvents.OnShowUpgradePanel.RemoveListener(ShowPanel);
        GlobalEvents.OnHideUpgradePanel.RemoveListener(HidePanel);

    }

    private void OnDestroy()
    {
        GlobalEvents.OnHideUpgradePanel.RemoveListener(HidePanel);
        GlobalEvents.OnShowUpgradePanel.RemoveListener(ShowPanel); // safety net
    }

    public override void ShowPanel()
    {
        if (this == null) return;

        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public override void HidePanel()
    {
        if (this == null) return;

        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

    }
}
