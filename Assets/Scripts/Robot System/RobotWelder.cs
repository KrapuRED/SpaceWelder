using UnityEngine;

public class RobotWelder : Robot, IDamageAble
{
    [Header("Robot Welder Config")]
    [SerializeField] private float _maxHealth;
    [SerializeField] private float _currentHealth;
    [SerializeField] private BoomArmGenerator boomArmGenerator;

    public float MaxHealth => _maxHealth;
    public float CurrentHealth => _currentHealth;

    private void Start()
    {
        _currentHealth = _maxHealth;

        GlobalEvents.OnUpdateHealthRobotUI.Invoke(_currentHealth);
    }

    private void OnEnable()
    {
        GlobalEvents.OnApplyExtraBoomArm.AddListener(OnGetExtraBoom);
    }

    private void OnDisable()
    {
        GlobalEvents.OnApplyExtraBoomArm.RemoveListener(OnGetExtraBoom);

    }

    private void OnDestroy()
    {
        GlobalEvents.OnApplyExtraBoomArm.RemoveListener(OnGetExtraBoom);

    }

    public void OnTakeDamage(float damageValue)
    {
        _currentHealth -= damageValue;
        GlobalEvents.OnUpdateHealthRobotUI.Invoke(_currentHealth/100f);

        if (_currentHealth <= 0)
        {
            Debug.Log("[RobotWelder] Dead!");
            Destroy(gameObject);
        }

        Debug.Log($"[RobotWelder] current Health = {_currentHealth}");
    }

    void OnGetExtraBoom(int extraBoom)
    {
        if (boomArmGenerator == null)
        {
            Debug.Log($"Boom Arm Generator are missing from {gameObject.name}");
            return;
        }
        Debug.Log("Extra boom : " +  extraBoom);

        for (int i = 0; i < extraBoom; i++)
        {
            boomArmGenerator.AddArm();
        }
    }
}
