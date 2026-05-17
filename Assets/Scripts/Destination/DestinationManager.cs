using UnityEngine;

public class DestinationManager : MonoBehaviour
{
    [Header("Destination Manager Config")]
    [SerializeField] private float _speedShip;
    [SerializeField] private float _maxSpeedShip;
    [SerializeField] private float _acceleration;
    [SerializeField] private float _distanceToTravel;

    private EffciencyShipManager _effciencyShipManager;
    private float _currentSpeed;
    private float _distanceTravel;
    private float _efficiency;
    private bool _reachDestination;
    private float _timeElapsed;

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
        _timeElapsed += Time.deltaTime;

        GlobalEvents.OnProgressDestinationUI.Invoke(_distanceTravel, _distanceToTravel);
        GlobalEvents.OnProgressTimeDestinationUI.Invoke(_timeElapsed);
    }

    private void ReachDestination()
    {
        _reachDestination = true;
        Debug.Log($"Ship reach the Destination in {_timeElapsed}");
    }

    private void CalculateShipToDestination()
    {
        float prevEfficiency = _efficiency;
        _efficiency = _effciencyShipManager.EffciencyShip / 100f;

        if (prevEfficiency != _efficiency)
            Debug.Log($"Efficiency changed: {prevEfficiency} -> {_efficiency}");

        float targetSpeed = Mathf.Min(_speedShip * _efficiency, _maxSpeedShip   ) ;
        _currentSpeed = Mathf.MoveTowards(_currentSpeed, targetSpeed, _acceleration * Time.deltaTime);

        _distanceTravel += _currentSpeed * Time.deltaTime;

        //Debug.Log($"Efficiency: {_efficiency} | Target: {targetSpeed} | Current: {_currentSpeed}");
    }
}
