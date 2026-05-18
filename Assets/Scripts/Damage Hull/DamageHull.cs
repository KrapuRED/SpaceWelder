using UnityEngine;

public class DamageHull : MonoBehaviour, IRepairAble
{
    [Header("Hull Damage Configure")]
    [SerializeField] private string _hullID;
    [SerializeField] private float _maxHealth;
    [SerializeField] private bool _isHullBreach;
    public Transform WeldingEffectContiner;
    private float _currentHealth;

    private SpriteRenderer _spriteRenderer;
    
    public string HullID => _hullID;
    public bool IsHullBreach => _isHullBreach;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void OnHullBreach(Sprite hullBreachSprite)
    {
        _currentHealth = 0;
        _isHullBreach = true;
        _spriteRenderer.sprite = hullBreachSprite;
    }

    public void OnReapairHull(float repairAmount)
    {
        if (!_isHullBreach) return;

        _currentHealth = Mathf.Abs(Mathf.Min(_currentHealth + repairAmount, _maxHealth));

        if (_currentHealth >= _maxHealth)
        {
            _isHullBreach = false;
            GlobalEvents.OnHullBeenReapir.Invoke(HullID);
            _spriteRenderer.sprite = null;

            foreach (Transform child in WeldingEffectContiner)
            {
                Destroy(child.gameObject);
            }
        }

        Debug.Log($"{gameObject.name} : {_currentHealth}/{_maxHealth}");
    }
}
