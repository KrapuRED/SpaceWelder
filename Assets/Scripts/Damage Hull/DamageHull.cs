using UnityEngine;

public class DamageHull : MonoBehaviour, IRepairAble
{
    [Header("Hull Damage Configure")]
    [SerializeField] private GameObject _explosionVFX;
    [SerializeField] private Transform _explosionVFXContainer;
    [SerializeField] private string _hullID;
    [SerializeField] private float _maxHealth;
    [SerializeField] private bool _isHullBreach;
    [SerializeField] private bool _activeAtStart;
    public Transform WeldingEffectContiner;
    private float _currentHealth;

    private SpriteRenderer _spriteRenderer;
    
    public string HullID => _hullID;
    public bool IsHullBreach => _isHullBreach;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        GlobalEvents.OnStartHullBreachGame.AddListener(HullBreachAtStart);
    }

    private void OnDisable()
    {
        GlobalEvents.OnStartHullBreachGame.RemoveListener(HullBreachAtStart);
    }

    private void HullBreachAtStart()
    {
        if (_activeAtStart)
        {
            ManagerHullShip.Insantce.OnStartHullBreach(HullID);
        }
    }

    public void OnHullBreach(Sprite hullBreachSprite)
    {
        ClearEffect();
        PlayVFX();

        _currentHealth = 0;
        _isHullBreach = true;
        _spriteRenderer.sprite = hullBreachSprite;
        SoundEffectManager.Instance.PlaySoundEffect("Explosion");
    }

    public void OnReapairHull(float repairAmount)
    {
        if (!_isHullBreach) return;

        _currentHealth = Mathf.Abs(Mathf.Min(_currentHealth + repairAmount, _maxHealth));

        if (_currentHealth >= _maxHealth)
        {
            _isHullBreach = false;
            SoundEffectManager.Instance.PlaySoundEffect("HullReapir");
            GlobalEvents.OnHullBeenReapir.Invoke(HullID);
            _spriteRenderer.sprite = null;

            ClearEffect();
        }
    }

    private void PlayVFX()
    {
        if (_explosionVFXContainer.childCount > 1)
        {
            var explsionVFX = _explosionVFXContainer.GetChild(0).GetComponent<ExplosionVFX>();
            if (explsionVFX != null)
            {
                explsionVFX.PlayExplosion();
                return;
            }
        }

        GameObject explosion = Instantiate(
           _explosionVFX,
           _explosionVFXContainer.position,
           Quaternion.identity,
           _explosionVFXContainer);

        explosion.GetComponent<ExplosionVFX>()?.PlayExplosion();
    }

    private void ClearEffect()
    {
        foreach (Transform child in WeldingEffectContiner)
        {
            Destroy(child.gameObject);
        }
    }
}
