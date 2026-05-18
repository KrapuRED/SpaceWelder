using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PanelPerformance : Panel
{
    private void OnEnable()
    {
        GlobalEvents.OnShowPerformacePanel.AddListener(ShowPerformance);
    }

    private void OnDisable()
    {
        GlobalEvents.OnShowPerformacePanel.RemoveListener(ShowPerformance);

    }

    private void OnDestroy()
    {
        GlobalEvents.OnShowPerformacePanel.RemoveListener(ShowPerformance);

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

    public void ShowPerformance(List<MissionSuccesData> missionSuccesDatas)
    {
        if (this == null) return;

        ShowPanel();

        foreach (var missionSucces in missionSuccesDatas)
        {
            string timeString = TimeSpan.FromSeconds(missionSucces.ArriveAt).ToString(@"hh\:mm\:ss");

            Debug.Log($"Mission Name: {missionSucces.MissionName} Mission ArriveAt : {timeString} Mission : {missionSucces.MissionSuccesType}");
        }
    }
}
