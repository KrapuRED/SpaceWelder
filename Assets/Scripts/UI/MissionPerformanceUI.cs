using UnityEngine;
using TMPro;
using System;

public class MissionPerformanceUI : MonoBehaviour
{
    public TextMeshProUGUI missionNameText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI resusltText;

    public void Setup(MissionSuccesData data)
    {
        string timeString = TimeSpan.FromSeconds(data.ArriveAt).ToString(@"hh\:mm\:ss");

        missionNameText.text = data.MissionName;
        timeText.text = timeString;
        resusltText.text = data.MissionSuccesType.ToString().ToUpper();
    }
}
