using UnityEngine;

public class DamageHull : MonoBehaviour, IRepairAble
{
    [Header("Hull Damage Configure")]
    [SerializeField] private float _maxHealth;
    [SerializeField] private float _repairMultiplier;
    public Transform WeldingEffectContiner;
    private float _currentHealth;

    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _currentHealth = 0;
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void OnReapairHull(float repairAmount)
    {
        _currentHealth = Mathf.Abs(Mathf.Min(_currentHealth + repairAmount * _repairMultiplier, _maxHealth));

        if (_currentHealth >= _maxHealth)
            Destroy(gameObject);

        Debug.Log($"{gameObject.name} Repaired! HP: {_currentHealth}/{_maxHealth}");
    }
}
