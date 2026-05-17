using UnityEngine;
using UnityEngine.UI;

public class ShipEffciencyStatusUI : MonoBehaviour
{
    public Image imgShipEffciencyStatus;

    private void OnEnable()
    {
        GlobalEvents.OnShipEffciencyUI.AddListener(UpdateShipEffciencyStatus);
    }

    private void OnDisable()
    {
        GlobalEvents.OnShipEffciencyUI.RemoveListener(UpdateShipEffciencyStatus);
    }

    public void UpdateShipEffciencyStatus(float effciencyValue)
    {
        if (imgShipEffciencyStatus == null) return;

        imgShipEffciencyStatus.fillAmount = effciencyValue;
    }
}
