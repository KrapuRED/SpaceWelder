using UnityEngine;
using UnityEngine.InputSystem;

public class RobotMovement : MonoBehaviour
{
    [Header("Movement Robot Config")]
    [SerializeField] private RobotWelder _ownerRobot;
    [SerializeField] private float _speedMovement = 5f;

    private Rigidbody2D _rigidbody2D;

    private void Awake()
    {
        _rigidbody2D = GetComponentInParent<Rigidbody2D>();
    }

    public void OnMovementCharacter(InputAction.CallbackContext contex)
    {
        if (!_ownerRobot.IsStillInRail)
        {
            Debug.LogWarning("Robot is not in the rail, cannot move.");
            return;
        }

        if (contex.performed)
        {
            Vector2 direction = contex.ReadValue<Vector2>();
            _rigidbody2D.AddForce(direction * _speedMovement, ForceMode2D.Force);
        }
        else if (contex.canceled)
        {
            _rigidbody2D.linearVelocity = Vector2.zero;
        }
    }
}
