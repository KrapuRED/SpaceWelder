using UnityEngine;

public class RobotUpgradeManager : MonoBehaviour, IDataPersistence
{
    public static RobotUpgradeManager Instance { get; private set; }

    private int extraBoomArm;
    private float speedUpgrade;
    private float weldingAreaUpgrade;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

      
    }

    private void OnEnable()
    {
        GlobalEvents.OnUpgradeRobot.AddListener(OnButtonUpgradePress);
    }

    private void OnDisable()
    {
        GlobalEvents.OnUpgradeRobot.RemoveListener(OnButtonUpgradePress);

    }

    void OnButtonUpgradePress(UpgradeType type, float amount)
    {
        switch (type)
        {
            case UpgradeType.ExtraBoom:
                UpgradeBoomArm((int) amount);
                break;

            case UpgradeType.Speed:
                UpgradeSpeed(amount);
                return;

            case UpgradeType.WeldingRadius:
                UpgradeWeldingRadius(amount);
                break;
        }

    }

    public bool UpgradeBoomArm(int amount)
    {
        extraBoomArm += amount;
        DataPersistenceManager.Instance.SaveGame();
        return true;
    }

    public bool UpgradeSpeed(float amount)
    {
        speedUpgrade += amount;
        DataPersistenceManager.Instance.SaveGame();
        return true;
    }

    public bool UpgradeWeldingRadius(float amount)
    {
        weldingAreaUpgrade += amount;
        DataPersistenceManager.Instance.SaveGame();
        return true;
    }

    public void ApplyUpgrades()
    {
        GlobalEvents.OnApplyExtraBoomArm.Invoke(extraBoomArm);
        GlobalEvents.OnApplySpeedUpgrade.Invoke(speedUpgrade);
        GlobalEvents.OnApplyWeldingUpgrade.Invoke(weldingAreaUpgrade);
    }

    public void LoadData(GameData data)
    {
        this.extraBoomArm       = data.extraBoomArm;
        this.speedUpgrade       = data.speedUpgrade;
        this.weldingAreaUpgrade = data.weldingAreaUpgrade;
    }

    public void SaveData(ref GameData data)
    {
        data.extraBoomArm       = this.extraBoomArm;
        data.speedUpgrade       = this.speedUpgrade;
        data.weldingAreaUpgrade = this.weldingAreaUpgrade;
    }
}
