using System;
using UnityEngine;

public class RobotUpgradeManager : MonoBehaviour, IDataPersistence
{
    public static RobotUpgradeManager Instance { get; private set; }

    [SerializeField] private int extraBoomArm;
    [SerializeField] private float speedUpgrade;
    [SerializeField] private float weldingAreaUpgrade;
    
    [Header("Test Config")]
    [SerializeField] private bool applyUpgradeOnStart;


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

    private void Start()
    {
        if (applyUpgradeOnStart)
        {
            ApplyUpgrades();
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
        return true;
    }

    public bool UpgradeSpeed(float amount)
    {
        speedUpgrade += amount;
        return true;
    }

    public bool UpgradeWeldingRadius(float amount)
    {
        weldingAreaUpgrade += amount;
        return true;
    }

    public void ApplyUpgrades()
    {
        Debug.LogWarning($"Applying Upgrades: ExtraBoomArm={extraBoomArm}, SpeedUpgrade={speedUpgrade}, WeldingAreaUpgrade={weldingAreaUpgrade}");

        GlobalEvents.OnApplyExtraBoomArm.Invoke(extraBoomArm);
        GlobalEvents.OnApplySpeedUpgrade.Invoke(speedUpgrade);
        GlobalEvents.OnApplyWeldingUpgrade.Invoke(weldingAreaUpgrade);
    }

    public void LoadData(GameData data)
    {
        this.extraBoomArm       = data.extraBoomArm;
        this.speedUpgrade       = data.speedUpgrade;
        this.weldingAreaUpgrade = data.weldingAreaUpgrade;

        ApplyUpgrades();
    }

    public void SaveData(ref GameData data)
    {
        data.extraBoomArm       = this.extraBoomArm;
        data.speedUpgrade       = this.speedUpgrade;
        data.weldingAreaUpgrade = this.weldingAreaUpgrade;
    }
}
