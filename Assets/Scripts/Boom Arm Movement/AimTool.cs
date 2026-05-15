using UnityEngine;

public class AimTool : MonoBehaviour
{
    public Transform target;

    [SerializeField] private float rotationOffset;
    [SerializeField] private float rotateSpeed;

    void LateUpdate()
    {
        Vector3 dir = target.position - transform.position;

        float angle =
            Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        Quaternion targetRotation = Quaternion.Euler(0, 0, angle + rotationOffset);

        transform.localRotation = Quaternion.RotateTowards(
                                transform.localRotation,
                                targetRotation,
                                rotateSpeed * Time.deltaTime
                    );
    }
}
