using UnityEngine;
using UnityEngine.InputSystem;

public class MoveTarget : MonoBehaviour
{
    private Camera _cam;

    private void Awake()
    {
        _cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        transform.position = worldPos;
    }

}
