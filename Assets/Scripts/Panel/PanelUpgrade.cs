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
        canvasGroup.alpha = 1;
    }

    public override void HidePanel()
    {
        canvasGroup.alpha = 0;
    }
}
