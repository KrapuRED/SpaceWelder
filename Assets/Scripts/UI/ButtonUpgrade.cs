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

    public void OnClickButtonUpgrade()
    {
        GlobalEvents.OnUpgradeRobot.Invoke(upgradeType);
    }
}
