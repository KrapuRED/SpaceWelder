using UnityEngine;

public class JoinPoint : MonoBehaviour
{
    public JoinPoint jp_child;

    public JoinPoint GetJoinPointChild()
    {
        return jp_child;
    }

    public void RotateJoinPoint(float angle)
    {
        transform.Rotate(-Vector3.back * angle);
    }
}
