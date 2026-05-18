using UnityEngine;

public enum UpgradeType
{
    ExtraBoom,
    Speed,
    WeldingRadius
}

public class ButtonUpgrade : MonoBehaviour
{
    public UpgradeType upgradeType;
    public float amount;

    public void OnClickButtonUpgrade()
    {
        GlobalEvents.OnUpgradeRobot.Invoke(upgradeType, amount);
    }
}
