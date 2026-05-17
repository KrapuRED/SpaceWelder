using UnityEngine;
using UnityEngine.UI;

public class RobotHealthUI : MonoBehaviour
{
    public Slider HealthSlider;

    private void OnEnable()
    {
        GlobalEvents.OnUpdateHealthRobotUI.AddListener(UpdateHealthSlider);
    }

    private void OnDisable()
    {
        GlobalEvents.OnUpdateHealthRobotUI.RemoveListener(UpdateHealthSlider);
    }

    public void UpdateHealthSlider(float healthValue)
    {
        HealthSlider.value = healthValue;
    }
}
