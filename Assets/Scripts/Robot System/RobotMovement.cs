using UnityEngine;
using UnityEngine.InputSystem;

public class RobotMovement : MonoBehaviour
{
    [Header("Movement Robot Config")]
    [SerializeField] private RobotWelder _ownerRobot;
    [SerializeField] private RailPointMarker _startPoint;
    [SerializeField] private float _speedMovement = 5f;
    [SerializeField] private float _nearTargetPoint = 0.05f;
    [SerializeField] private RailPoint _currentPoint;
    [SerializeField] private Direction _currentDirection;
    [SerializeField] private float _baseSpeedRate = 1f;
    private float _speedMultiplier = 1f;

    [Header("Junction Pause Config")]
    [SerializeField] private float _junctionPauseDuration = 0.25f; // Adjust between 0.1s and 0.5s
    private bool _isPausedAtJunction = false;
    private float _pauseTimer = 0f;

    private RailPoint _targetPoint;
    [SerializeField] private Direction? _bufferedInput;
    [SerializeField] private bool _isMoving;

    private void Start()
    {
        if (_startPoint != null)
            _currentPoint = _startPoint.GetRailPoint();

        if (_currentPoint == null )
        {
            Debug.LogError($"The Current point on {gameObject.name} is NULL!");
            return;
        }

        transform.position = _currentPoint.position;
    }

    private void OnEnable()
    {
        GlobalEvents.OnApplySpeedUpgrade.AddListener(UpgradeSpeed);
    }

    private void OnDisable()
    {
        GlobalEvents.OnApplySpeedUpgrade.RemoveListener(UpgradeSpeed);

    }

    private void UpgradeSpeed(float percent)
    {
        _speedMultiplier = _baseSpeedRate + (percent / 100f);
        Debug.Log("Welding been upgrade to " + _speedMultiplier);
    }

    public void InputMovementRail(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            SoundEffectManager.Instance.PlaySoundEffect("Move-Start");
        }

        if (context.performed)
        {
            Vector2 dir = context.ReadValue<Vector2>();
            _bufferedInput = GetInputDirection(dir);
        }

        if (context.canceled)
        {
            _bufferedInput = null;
            if(!_isMoving)
                StopMoving();
        }
    }

    private void Update()
    {
        if (_isPausedAtJunction)
        {
            Debug.Log("Paused at junction, timer: " + _pauseTimer);
            _pauseTimer -= Time.deltaTime;
            _isMoving = false; // Ensure movement is blocked while paused
            if (_pauseTimer <= 0f)
            {
                _isPausedAtJunction = false;
                if (_bufferedInput != null)
                {
                    SoundEffectManager.Instance.PlaySoundEffectLoop("Move-Constant");
                }
            }
            return; // Block movement logic while paused
        }

        if (_isMoving)
        {
            MoveToRailPoint();
        }
        else
            TryToMove();
    }

    private void StartMoving()
    {
        _isMoving = true;
        SoundEffectManager.Instance.PlaySoundEffectLoop("Move-Constant");
    }

    private void StopMoving()
    {
        _isMoving = false;
        SoundEffectManager.Instance.StopSoundEffectLoop("Move-Constant");
        SoundEffectManager.Instance.PlaySoundEffect("Move-End");
    }

    private void TryToMove()
    {
        if (_currentPoint == null) return;
        if (_bufferedInput == null) return;

        Direction input = _bufferedInput.Value;

        if (_currentPoint.connections.TryGetValue(input, out RailPoint next))
        {
            _currentDirection = input;
            _targetPoint = next;
            StartMoving();
            return;
        }
    }

    private void MoveToRailPoint()
    {
        if (_targetPoint == null) return;

        transform.position = Vector2.MoveTowards(
            transform.position,
            _targetPoint.position,
             _speedMovement * _speedMultiplier * Time.deltaTime);

        if (Vector2.Distance(transform.position, _targetPoint.position) < _nearTargetPoint)
        {
            transform.position = _targetPoint.position;
            _currentPoint = _targetPoint;

            if (_currentPoint.connections != null && _currentPoint.connections.Count > 2)
            {
                _isPausedAtJunction = true;
                _pauseTimer = _junctionPauseDuration;

                // Temporarily stop the constant movement sound while paused
                SoundEffectManager.Instance.StopSoundEffectLoop("Move-Constant");
            }

            if (_bufferedInput == null)
                StopMoving();
            else
                TryToMove();
        }
    }

    private Direction? GetInputDirection(Vector2 direction)
    {
        // Fixed: nullable return for zero vector case
        if (direction.x < 0) return Direction.Left;
        if (direction.x > 0) return Direction.Right;
        if (direction.y < 0) return Direction.Down;
        if (direction.y > 0) return Direction.Up;
        return null;
    }
}
