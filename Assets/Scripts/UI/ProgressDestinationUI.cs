using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressDestinationUI : MonoBehaviour
{
    public Slider ProgressDestinationSlider;
    public TextMeshProUGUI TimeText;

    private void OnEnable()
    {
        GlobalEvents.OnProgressDestinationUI.AddListener(UpdateProgressSlider);
        GlobalEvents.OnProgressTimeDestinationUI.AddListener(UpdateTimeProgress);
    }

    private void OnDisable()
    {
        GlobalEvents.OnProgressDestinationUI.RemoveListener(UpdateProgressSlider);
        GlobalEvents.OnProgressTimeDestinationUI.RemoveListener(UpdateTimeProgress);

    }

    public void UpdateProgressSlider(float progressValue, float distance)
    {
        ProgressDestinationSlider.value = progressValue/distance;
    }

    public void UpdateTimeProgress(float timeValue)
    {
        string timeString = TimeSpan.FromSeconds(timeValue).ToString(@"hh\:mm\:ss");

        TimeText.text = timeString;
    }

}
