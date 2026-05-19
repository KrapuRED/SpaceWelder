using UnityEngine;

public class DestinationManager : MonoBehaviour
{
    public static DestinationManager Instance;

    [Header("Destination Manager Config")]
    [SerializeField] private float _speedShip;
    [SerializeField] private float _maxSpeedShip;
    [SerializeField] private float _acceleration;
    [SerializeField] private float _distanceToTravel;

    private EffciencyShipManager _effciencyShipManager;
    private MissionControlManager _missionControlManager;
    private float _currentSpeed;
    private float _distanceTravel;
    private float _efficiency;
    private bool _reachDestination;
    private float _timeElapsed;

    public float TimeElapsed => _timeElapsed;
    public float TotalDistance => _distanceTravel;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        _effciencyShipManager = EffciencyShipManager.Instance;
    }

    private void Update()
    {
        if (_reachDestination) return;

        if (_distanceTravel >= _distanceToTravel)
        {
            ReachDestination();
            return;
        }

        CalculateShipToDestination();

        if (_efficiency > 0f)                                          
            _timeElapsed += Time.deltaTime * (1f / _efficiency);
        else
            _timeElapsed += Time.deltaTime;

        ManagerHullShip.Insantce.CheckPhase(_timeElapsed);
        GlobalEvents.OnProgressDestinationUI.Invoke(_distanceTravel, _distanceToTravel);
        GlobalEvents.OnProgressTimeDestinationUI.Invoke(_timeElapsed);
    }

    public float DistanceToTarget()
    {
        return _distanceToTravel - _distanceTravel;
    }

    private void ReachDestination()
    {
        _reachDestination = true;

        MissionControlManager.Instance.OnReachDestination(_timeElapsed);
        GlobalEvents.OnShowUpgradePanel.Invoke();
        GlobalEvents.OnReachDestination.Invoke();
    }

    private void CalculateShipToDestination()
    {
        float prevEfficiency = _efficiency;
        _efficiency = _effciencyShipManager.EfficiencyShip / 100f;

        float targetSpeed = Mathf.Min(_speedShip * _efficiency, _maxSpeedShip);
        _currentSpeed = Mathf.MoveTowards(_currentSpeed, targetSpeed, _acceleration * Time.deltaTime);
        _distanceTravel += _currentSpeed * Time.deltaTime;
    }
}
