using UnityEngine;

public class IKBoomArmManager : MonoBehaviour
{
    [Header("IK Boom Arm Configure")]
    [SerializeField] private float deltaTheta;
    [SerializeField] private float rotateThreshold;
    [SerializeField] private float rotate_rate;
    [SerializeField] private int steps;

    public JoinPoint jp_root;
    public JoinPoint jp_end;

    public GameObject target;

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
}
