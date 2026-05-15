using UnityEngine;
using UnityEngine.InputSystem;

public class CursorIKTarget : MonoBehaviour
{
    public enum Mode
    {
        WorldPlane,
        Raycast
    }

    [Header("Tracking Medo")]
    public Mode trackingMode = Mode.WorldPlane;

    [Header("WorldPlane settings")]
    [Tooltip("Normal of the plane the cursor projects onto.\n" +
             "Use Vector3.right for a side-scrolling view.\n" +
             "Use Vector3.forward for a top-down view.")]
    public Vector3 planeNormal = Vector3.forward;

    [Tooltip("A point on the plane. Drag your robot's root bone here.")]
    public Transform planeOrigin;

    [Header("Raycast settings")]
    [Tooltip("Layers the raycast can hit (ground, platforms, etc.)")]
    public LayerMask raycastLayers = ~0;
    [Tooltip("Fallback height if nothing is hit.")]
    public float fallbackHeight = 0f;

    [Header("Smoothing")]
    [Range(1f, 40f)] public float smoothSpeed = 20f;

    [Header("Reach clamp (optional)")]
    [Tooltip("Drag the FABRIKSolver here to auto-clamp target within arm reach.")]
    public FABRIKSolver solver;
    [Tooltip("The root bone / shoulder position used as clamp center.")]
    public Transform armRoot;

    Camera _cam;
    Vector3 _desire;

    private void Awake()
    {
        _cam = Camera.main;
        _desire = transform.position;
    }

    void Update()
    {
        _desire = trackingMode == Mode.WorldPlane ? GetPlanePos() : GetRaycast();
        ClampToReach();

        transform.position = Vector3.Lerp(transform.position, _desire, Time.deltaTime * smoothSpeed);
    }

    Vector3 GetPlanePos()
    {
        Vector3 origin = planeOrigin ? planeOrigin.position : Vector3.zero;
        var plane      = new Plane(planeNormal.normalized, origin);
        Ray ray        = _cam.ScreenPointToRay(Mouse.current.position.ReadValue());

        return plane.Raycast(ray, out float d) ? ray.GetPoint(d) : transform.position;
    }

    Vector3 GetRaycast()
    {
        Debug.Log("Raycast mode not implemented yet.");
        return Vector3.zero;
    }

    void ClampToReach()
    {
        if (solver == null || armRoot == null) return;

        Vector3 center = armRoot.position;
        Vector3 offset = _desire - center;
        float maxRange = solver.GetTotalLength();

        if (offset.magnitude > maxRange)
        {
            _desire = center + offset.normalized * maxRange;
        }
    }
}
