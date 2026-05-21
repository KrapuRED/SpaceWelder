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
        SoundEffectManager.Instance.PlaySoundEffect("ClickBottom");
        GlobalEvents.OnUpgradeRobot.Invoke(upgradeType, amount);
        GlobalEvents.OnHideUpgradePanel.Invoke();
        MissionControlManager.Instance.OnShowPerformace();
    }
}
