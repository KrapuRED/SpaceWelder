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
    public int nextSceneID;

    public void OnClickButtonUpgrade()
    {
        Debug.Log($"[{gameObject.name}] OnClickButtonUpgrade");
        GlobalEvents.OnUpgradeRobot.Invoke(upgradeType, amount);
        GameManager.Instance.NextLevel(nextSceneID);
    }
}
