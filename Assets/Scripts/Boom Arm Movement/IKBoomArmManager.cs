using System.Collections.Generic;
using UnityEngine;

public class IKBoomArmManager : MonoBehaviour
{
    [Header("IK Boom Arm Configure")]
    [SerializeField] private float deltaTheta;
    [SerializeField] private float rotateThreshold;
    [SerializeField] private float rotate_rate;
    [SerializeField] private int steps;

    [SerializeField] private JoinPoint jp_root;
    [SerializeField] private JoinPoint jp_end;

    public JoinPoint JointPointRoot => jp_root;
    public JoinPoint JointPoinEnd => jp_end;
    public GameObject target;

    public bool IsRebuilding = false;

    private void Start()
    {
        BuildChainFromHierarchy();
    }

    public void RebuildChain()
    {
        IsRebuilding = true;
        BuildChainFromHierarchy();
        IsRebuilding = false;
    }

    private void BuildChainFromHierarchy()
    {
        List<JoinPoint> joints = new List<JoinPoint>();

        CollectJoints(jp_root, joints);

        for (int i = 0; i < joints.Count - 1; i++)
        {
            joints[i].jp_child = joints[i + 1];
        }

        if (joints.Count > 0)
        {
            joints[joints.Count - 1].jp_child = null;
            jp_end = joints[joints.Count - 1]; // auto-assign end
        }

        Debug.Log($"IK Chain built: {joints.Count} joints. End = {jp_end?.name}");
    }

    private void CollectJoints(JoinPoint current, List<JoinPoint> result)
    {
        if (current == null) return;
        result.Add(current);

        // Find the next JoinPoint among children in scene hierarchy
        foreach (Transform child in current.transform)
        {
            JoinPoint next = child.GetComponent<JoinPoint>();
            if (next != null)
            {
                CollectJoints(next, result);
                break; // Only follow first JoinPoint child (linear chain)
            }
        }
    }

    float GetCalculated(JoinPoint _joinPoint)
    {
        float distance1 = GetDistance(jp_end.transform.position, target.transform.position);
        _joinPoint.RotateJoinPoint(deltaTheta);

        float distance2 = GetDistance(jp_end.transform.position, target.transform.position);
        _joinPoint.RotateJoinPoint(-deltaTheta);

        return (distance2 - distance1) / deltaTheta;
    }

    private void Update()
    {
        if (IsRebuilding) return; // guard against destroyed objects
        if (jp_end == null || target == null) return; // null safety

        for (int i = 0; i < steps; ++i)
        {
            if (GetDistance(jp_end.transform.position, target.transform.position) > rotateThreshold)
            {
                JoinPoint current = jp_root;

                while (current != null)
                {
                    float slope = GetCalculated(current);
                    current.RotateJoinPoint(-slope * rotate_rate);
                    current = current.GetJoinPointChild();
                }
            }
        }
    }

    float GetDistance(Vector3 point1, Vector3 point2)
    {
        return Vector3.Distance(point1, point2);
    }

    public void SetChain(JoinPoint root, JoinPoint end)
    {
        Debug.Log($"Setting IK chain: Root = {root?.name}, End = {end?.name}");
        jp_root = root;
        jp_end = end;
    }
}
