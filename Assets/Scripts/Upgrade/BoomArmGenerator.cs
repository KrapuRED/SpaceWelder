using UnityEngine;

public class BoomArmGenerator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private IKBoomArmManager ikManager;
    [SerializeField] private GameObject joinPrefab;  // prefab with JoinPoint component
    [SerializeField] private GameObject armPrefab;   // the visual arm mesh

    [Header("Spawn Settings")]
    [SerializeField] private Vector3 jointOffset = new Vector3(0, -1f, 0);

    public void AddArm()
    {
        JoinPoint joinLastPoint = FindTheDeepPoint(ikManager.JointPointRoot);
        if (joinLastPoint == null) return;

        Transform parentOfTip = joinLastPoint.transform.parent;

        joinLastPoint.transform.SetParent(null);

        GameObject newArm = Instantiate(armPrefab, parentOfTip.transform);
        newArm.name = "Arm";

        Transform startPoint = newArm.transform.Find("startPoint");
        Transform endPoint = newArm.transform.Find("endPoint");

        if (startPoint == null || endPoint == null)
        {
            Debug.LogError("Arm prefab missing startPoint or endPoint!");
            Destroy(newArm);
            return;
        }

        Vector3 offset = parentOfTip.position - startPoint.position;
        newArm.transform.position += offset;

        newArm.transform.SetParent(parentOfTip, worldPositionStays: true);

        GameObject newJoin = Instantiate(joinPrefab, parentOfTip);
        newJoin.name = $"Join {GetJointCount(ikManager.JointPointRoot)}";
        newJoin.transform.position = endPoint.position;
        newJoin.transform.SetParent(parentOfTip, worldPositionStays: true);

        joinLastPoint.transform.SetParent(newJoin.transform, worldPositionStays: true);
        joinLastPoint.transform.localPosition = Vector3.zero;

        ikManager.RebuildChain();

        Debug.Log($"New arm added! New joint: {newJoin.name}");
    }

    private JoinPoint FindTheDeepPoint(JoinPoint current)
    {
        if (current == null) return null;

        foreach (Transform child in current.transform)
        {
            JoinPoint next = child.GetComponent<JoinPoint>();
            if (next != null)
                return FindTheDeepPoint(next);
        }

        return current;
    }

    private int GetJointCount(JoinPoint root)
    {
        int count = 0;
        JoinPoint current = root;
        while (current != null)
        {
            count++;
            current = current.GetJoinPointChild();
        }
        return count;
    }
}
