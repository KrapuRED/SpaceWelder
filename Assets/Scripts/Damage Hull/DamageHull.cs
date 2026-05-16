using UnityEngine;

public class DamageHull : MonoBehaviour, IRepairAble
{
    [Header("Hull Damage Configure")]
    [SerializeField] private string _hullID;
    [SerializeField] private float _maxHealth;
    [SerializeField] private float _repairMultiplier;
    [SerializeField] private bool _isHullBreach;
    public Transform WeldingEffectContiner;
    private float _currentHealth;

    private SpriteRenderer _spriteRenderer;
    
    public string HullID => _hullID;
    public bool IsHullBreach => _isHullBreach;

    private void Awake()
    {
        _currentHealth  = 0;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.color = Color.green;
    }

    public void OnHullBreach()
    {
        _isHullBreach = true;
        _spriteRenderer.color = Color.red;
    }

    public void OnReapairHull(float repairAmount)
    {
        if (!_isHullBreach) return;

        _currentHealth = Mathf.Abs(Mathf.Min(_currentHealth + repairAmount * _repairMultiplier, _maxHealth));

        if (_currentHealth >= _maxHealth)
        {
            _isHullBreach = false;
            GlobalEvents.OnHullBeenReapir.Invoke(HullID);
            _spriteRenderer.color = Color.green;

            foreach (Transform child in WeldingEffectContiner)
            {
                Destroy(child.gameObject);
            }
        }

        Debug.Log($"{gameObject.name} Repaired! HP: {_currentHealth}/{_maxHealth}");
    }
}
