using UnityEngine;
using UnityEngine.InputSystem;

public class Welding : MonoBehaviour
{
    [SerializeField] private Transform _weldingPoint;
    [SerializeField] private float _weldingRadius;
    [SerializeField] private LayerMask _hullDamageLayer;
    [SerializeField] private float _minMoveDistance;

    [SerializeField] private GameObject _particelWelding;

    private bool _isWelding;
    private Vector2 _lastWeldingPointPos;

    public void OnWelding(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _isWelding = true;
            _lastWeldingPointPos = _weldingPoint.position;
        }

        if (context.canceled)
        {
            _isWelding = false;
        }

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

                    damageHull.OnReapairHull(moveDistance);
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
