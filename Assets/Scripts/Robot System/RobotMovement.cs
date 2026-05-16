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

    private RailPoint _targetPoint;
    [SerializeField] private Direction? _bufferedInput;
    [SerializeField] private bool _isMoving;
    private Rigidbody2D _rigidbody2D;

    private void Start()
    {
        _currentPoint = _startPoint.GetRailPoint();

        if (_currentPoint == null )
        {
            Debug.LogError($"The Current point on {gameObject.name} is NULL!");
            return;
        }

        transform.position = _currentPoint.position;
    }

    public void InputMovementRail(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 dir = context.ReadValue<Vector2>();
            _bufferedInput = GetInputDirection(dir);
            Debug.Log($"[Input] performed: {_bufferedInput}");
        }

        if (context.canceled)
        {
            Debug.Log($"[Input] canceled, clearing buffer");
            _bufferedInput = null;
        }
    }

    private void Update()
    {
        if (_isMoving)
            MoveToRailPoint();
        else
            TryToMove();
    }

    private void TryToMove()
    {
        if (_currentPoint == null) return;
        if (_bufferedInput == null) return;

        Direction input = _bufferedInput.Value;

        Debug.Log($"[TryToMove] input: {input} | currentDir: {_currentDirection} | pointType: {_currentPoint.type} | available: {string.Join(", ", _currentPoint.availableDirections)}");

        if (_currentPoint.connections.TryGetValue(input, out RailPoint next))
        {
            _currentDirection = input;
            _targetPoint = next;
            _isMoving = true;
            return;
        }
        else
        {
            Debug.Log($"[TryToMove] {input} not found in connections: {string.Join(", ", _currentPoint.connections.Keys)}");
        }
    }

    private void MoveToRailPoint()
    {
        if (_targetPoint == null) return;

        transform.position = Vector2.MoveTowards(
            transform.position,
            _targetPoint.position,
            _speedMovement * Time.deltaTime);

        if (Vector2.Distance(transform.position, _targetPoint.position) < _nearTargetPoint)
        {
            transform.position = _targetPoint.position;
            _currentPoint = _targetPoint;
            _isMoving = false;
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
