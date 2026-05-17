using UnityEngine;

public class RobotUpgradeManager : MonoBehaviour
{
    public static RobotUpgradeManager Instance { get; private set; }

    [Header("Upgrade Data")]
    [SerializeField] private RobotUpgrade upgradeData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        upgradeData.Load();
    }

    private void OnEnable()
    {
        GlobalEvents.OnUpgradeRobot.AddListener(OnButtonUpgradePress);
    }

    private void OnDisable()
    {
        GlobalEvents.OnUpgradeRobot.RemoveListener(OnButtonUpgradePress);

    }

    void OnButtonUpgradePress(UpgradeType type)
    {
        switch (type)
        {
            case UpgradeType.ExtraBoom:
                UpgradeBoomArm();
                break;

            case UpgradeType.Speed:
                //UpgradeSpeed();
                return;

            case UpgradeType.WeldingRadius:
                //UpgradeWeldingRadius();
                break;
        }

    }

    public bool UpgradeBoomArm()
    {
        if (upgradeData.extraBoomArm >= upgradeData.maxBoomArm)
        {
            Debug.Log("Max boom arm reached!");
            return false;
        }

        upgradeData.extraBoomArm++;
        upgradeData.Save();
        return true;
    }

    public bool UpgradeSpeed(float amount)
    {
        if (upgradeData.speed >= upgradeData.maxSpeed)
        {
            Debug.Log("max Speed reached!");
            return false;
        }

        upgradeData.speed += amount;
        upgradeData.Save();
        return true;
    }

    public bool UpgradeWeldingRadius(float amount)
    {
        if (upgradeData.weldingArea >= upgradeData.maxWeldingArea)
        {
            Debug.Log("Max welding area reached!");
            return false;
        }

        upgradeData.weldingArea += amount;
        upgradeData.Save();
        return true;
    }

    public void ApplyUpgrades()
    {
        GlobalEvents.OnApplyExtraBoomArm.Invoke(upgradeData.extraBoomArm);

        Debug.Log($"Applied {upgradeData.extraBoomArm} boom arms, speed {upgradeData.speed}, weld area {upgradeData.weldingArea}");
    }

    public RobotUpgrade GetData() => upgradeData;
}
