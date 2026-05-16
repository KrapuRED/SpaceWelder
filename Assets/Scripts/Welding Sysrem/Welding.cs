using UnityEngine;
using UnityEngine.InputSystem;

public class Welding : MonoBehaviour
{
    [SerializeField] private Transform _weldingPoint;
    [SerializeField] private float _weldingRadius;
    [SerializeField] private LayerMask _hullDamageLayer;
    [SerializeField] private float _minMoveDistance;

    private bool _isWelding;
    private Vector2 _lastWeldingPointPos;

    public void OnWelding(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _isWelding = true;
            _lastWeldingPointPos = _weldingPoint.position;
            Debug.Log("[Welding - OnWelding] Started welding!");
        }

        if (context.canceled)
        {
            _isWelding = false;
            Debug.Log("[Welding - OnWelding] Stopped welding!");
        }

    }

    private void Update()
    {
        if (!_isWelding) return;

        float moveDistance = Vector2.Distance(_weldingPoint.position, _lastWeldingPointPos);

        if (moveDistance >= _minMoveDistance)
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(_weldingPoint.position, _weldingRadius, _hullDamageLayer);

            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.TryGetComponent(out IRepairAble reapairAble))
                {
                    reapairAble.OnReapairHull(moveDistance);
                }
            }

            _lastWeldingPointPos = _weldingPoint.position;
        }
    }
}
