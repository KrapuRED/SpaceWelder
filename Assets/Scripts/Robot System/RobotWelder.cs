using UnityEngine;

public class RobotWelder : Robot
{
    [Header("Robot Welder Config")]
    [SerializeField] private bool _isWelding = false;

    [Header("Rail")]
    [SerializeField] private bool _isStillInRail = false;
    [SerializeField] private Transform _checkRailPoint;
    [SerializeField] private float _checkRailRadius = 0.1f;
    [SerializeField] private LayerMask _layerRail;

    public bool IsWelding => _isWelding;
    public bool IsStillInRail => _isStillInRail;

    private void Awake()
    {
        Physics2D.IgnoreLayerCollision(
        gameObject.layer,                        
        gameObject.layer,                       
        true
    );
    }

    public void FixedUpdate()
    {
        CheckIfStillInRail();
    }

    private void CheckIfStillInRail()
    {
        Collider2D collider = Physics2D.OverlapCircle(_checkRailPoint.position, _checkRailRadius, _layerRail);
        _isStillInRail = collider != null;
    }
}
