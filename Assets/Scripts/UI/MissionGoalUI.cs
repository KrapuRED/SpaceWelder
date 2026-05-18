using System;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class MissionGoalUI : MonoBehaviour
{
    public TextMeshProUGUI MissionGoalUIText;

    private void OnEnable()
    {
        GlobalEvents.OnMissionGoalUI.AddListener(SetMissionGoalUI);
    }

    private void OnDisable()
    {
        GlobalEvents.OnMissionGoalUI.RemoveListener(SetMissionGoalUI);

    }

    private void OnDestroy()
    {
        GlobalEvents.OnMissionGoalUI.RemoveListener(SetMissionGoalUI);

    }

    public void SetMissionGoalUI(float timeValue)
    {
        if (this == null) return;

        string timeString = TimeSpan.FromSeconds(timeValue).ToString(@"hh\:mm\:ss");

        MissionGoalUIText.text = "Reach Destination Under " + timeString;
    }
}
