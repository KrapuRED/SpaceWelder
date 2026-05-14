using UnityEngine;

public class HandAim : MonoBehaviour
{
   public Transform target;
    public float rotateSpeed = 360f;

    void LateUpdate()
    {
        Vector2 dir = target.position - transform.position;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // Adjust this offset if the hand points wrong
        angle -= 90f;

        Quaternion targetRot = Quaternion.Euler(0, 0, angle);

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRot,
            rotateSpeed * Time.deltaTime
        );
    }
}