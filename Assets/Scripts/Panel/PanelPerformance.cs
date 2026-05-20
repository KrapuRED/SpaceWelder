using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PanelPerformance : Panel
{
    [SerializeField] private Transform contentParent;
    [SerializeField] private MissionPerformanceUI missionPrefab;
    [SerializeField] private TextMeshProUGUI performaceRatingText;
    [SerializeField] private PerfomanceRatingCalculate perfomanceRatingCalculate;

    private void OnEnable()
    {
        GlobalEvents.OnShowPerformancePanel.AddListener(ShowPerformance);
        GlobalEvents.OnHidePerformacnePanel.AddListener(HidePanel);
    }

    private void OnDisable()
    {
        GlobalEvents.OnShowPerformancePanel.RemoveListener(ShowPerformance);
        GlobalEvents.OnHidePerformacnePanel.RemoveListener(HidePanel);

    }

    private void OnDestroy()
    {
        GlobalEvents.OnShowPerformancePanel.RemoveListener(ShowPerformance);
        GlobalEvents.OnHidePerformacnePanel.RemoveListener(HidePanel);

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

        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }


        foreach (var missionSucces in missionSuccesDatas)
        {
            MissionPerformanceUI ui = Instantiate(missionPrefab, contentParent);

            ui.Setup(missionSucces);

            Debug.Log(
           $"Mission Name: {missionSucces.MissionName} " +
           $"Mission ArriveAt : {missionSucces.ArriveAt} " +
           $"Mission : {missionSucces.MissionSuccesType}"
       );
        }

        var performanceResult = perfomanceRatingCalculate.GetPerfomanceRatingByTime(missionSuccesDatas, missionSuccesDatas.Count);

        string performanceResultText = string.Empty;

        if (performanceResult != PerformanceRating.None)
        {
            switch (performanceResult)
            {
                case PerformanceRating.Excellent:
                    performanceResultText = performanceResult.ToString().ToLower();
                    break;

                case PerformanceRating.BitGood:
                    performanceResultText = "Bit Good".ToUpper();
                    break;

                case PerformanceRating.NotOkey:
                    performanceResultText = "NotOkey".ToUpper();
                    break;

                case PerformanceRating.Bad:
                    performanceResultText = performanceResult.ToString().ToLower();
                    break;
            }
        }

        performaceRatingText.text = performanceResultText;
        Debug.Log($"Performance Result : {performanceResultText} total missions : {missionSuccesDatas.Count}");
    }
}
