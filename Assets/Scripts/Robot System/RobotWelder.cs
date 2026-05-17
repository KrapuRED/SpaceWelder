using UnityEngine;

public class RobotWelder : Robot, IDamageAble
{
    [Header("Robot Welder Config")]
    [SerializeField] private float _maxHealth;
    [SerializeField] private float _currentHealth;

    private void Start()
    {
        _currentHealth = _maxHealth;
        GlobalEvents.OnUpdateHealthRobotUI.Invoke(_currentHealth);
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
}
