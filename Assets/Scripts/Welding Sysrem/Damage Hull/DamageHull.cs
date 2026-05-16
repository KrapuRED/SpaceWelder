using UnityEngine;

public class DamageHull : MonoBehaviour, IRepairAble
{
    [Header("Hull Damage Configure")]
    [SerializeField] private float _maxHealth;
    [SerializeField] private float _repairMultiplier;
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

        _spriteRenderer.size = new Vector2(_currentHealth , _currentHealth);

        Debug.Log($"{gameObject.name} Repaired! HP: {_currentHealth}/{_maxHealth}");
    }
}
