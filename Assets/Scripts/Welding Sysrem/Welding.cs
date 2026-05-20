using UnityEngine;
using UnityEngine.InputSystem;

public class Welding : MonoBehaviour
{
    [SerializeField] private RobotWelder _ownerRobot;
    [SerializeField] private Transform _weldingPoint;
    [SerializeField] private float _weldingRadius;
    [SerializeField] private LayerMask _hullDamageLayer;
    [SerializeField] private float _minMoveDistance;
    [SerializeField] private float _baseRepairRate = 1f;
    [SerializeField] private GameObject weldingParticle;
    private float _weldingMultiplier = 1f;

    [SerializeField] private GameObject _particelWelding;

    private bool _isWelding;
    private Vector2 _lastWeldingPointPos;

    public void OnWelding(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _isWelding = true;
            _lastWeldingPointPos = _weldingPoint.position;
            SoundEffectManager.Instance.PlaySoundEffectLoop("Welding");
        }
        if (context.canceled)
        {
            _isWelding = false;
            weldingParticle.SetActive(false);
            SoundEffectManager.Instance.StopSoundEffectLoop("Welding");
        }

    }

    private void OnEnable()
    {
        GlobalEvents.OnApplyWeldingUpgrade.AddListener(UpgradeWelding);
    }

    private void OnDisable()
    {
        GlobalEvents.OnApplyWeldingUpgrade.RemoveListener(UpgradeWelding);

    }

    private void UpgradeWelding(float percent)
    {
        _weldingMultiplier = _baseRepairRate + (percent / 100f);
    }

    private void Update()
    {
        if (!_isWelding) return;

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(_weldingPoint.position, _weldingRadius, _hullDamageLayer);

        float moveDistance = Vector2.Distance(_weldingPoint.position, _lastWeldingPointPos);

        if (moveDistance >= _minMoveDistance)
        {
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.TryGetComponent(out DamageHull damageHull))
                {
                    if (!damageHull.IsHullBreach) return;

                    weldingParticle.SetActive(true);
                    damageHull.OnReapairHull(_baseRepairRate * _weldingMultiplier);
                    WeldingHullEffect(damageHull.WeldingEffectContiner);
                }
            }

            _lastWeldingPointPos = _weldingPoint.position;
        }
    }

    private void WeldingHullEffect(Transform WeldingEffectContiner)
    {
        GameObject newParticle = Instantiate(_particelWelding, _weldingPoint.transform.position, Quaternion.identity);
        newParticle .transform.SetParent(WeldingEffectContiner, true);
    }
}
