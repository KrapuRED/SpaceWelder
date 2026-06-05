using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoomArmGenerator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private int baseLengthArm;
    [SerializeField] private IKBoomArmManager ikManager;
    [SerializeField] private GameObject welderPrefab; // prefab with Welder component
    [SerializeField] private GameObject joinPrefab;  // prefab with JoinPoint component
    [SerializeField] private GameObject armPrefab;   // the visual arm mesh

    [Header("Input")]
    [SerializeField] private PlayerInput playerInput;

    [Header("Spawn Settings")]
    [SerializeField] private Vector3 armDirection = new Vector3(0, 1f, 0);

    private int armCount = 0;

    public void AddArm(int extraBoom)
    {
        armCount = baseLengthArm + extraBoom;
        RebuildChain(armCount);
    }

    public void RemoveArm()
    {
        if (armCount <= 0) return;
        armCount--;
        RebuildChain(armCount);
    }
    private void RebuildChain(int totalArms)
    {
        ikManager.IsRebuilding = true; // set flag before destroying

        // Step 1: Destroy all children of Join 1
        JoinPoint root = ikManager.JointPointRoot;
        foreach (Transform child in root.transform)
        {
            if (child.CompareTag("DoNotDestroy"))
            {
                continue;
            }

            Destroy(child.gameObject);
        }

        if (totalArms <= 0)
        {
            ikManager.SetChain(root, root); // root is both start and end
            ikManager.IsRebuilding = false;
            return;
        }

        // Step 2: Build chain and track joints in order
        List<JoinPoint> joints = new List<JoinPoint>();
        joints.Add(root);

        Transform currentParent = root.transform;
        Vector3 currentWorldPos = root.transform.position;

        for (int i = 0; i < totalArms; i++)
        {
            // Spawn arm
            GameObject newArm = Instantiate(armPrefab, currentParent);
            newArm.name = $"Arm {i + 1}";
            newArm.transform.localScale = GetScaleToMatchWorld(currentParent);

            SpriteRenderer sr = newArm.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingOrder = i + 6; // Ensure each arm renders above the previous one
            }

            Transform startPoint = FindDeepChild(newArm.transform, "startPoint");
            Transform endPoint = FindDeepChild(newArm.transform, "endPoint");

            if (startPoint == null || endPoint == null)
            {
                Debug.LogError($"Arm prefab missing startPoint or endPoint on arm {i + 1}!");
                ikManager.IsRebuilding = false;
                return;
            }

            // Align startPoint to currentWorldPos
            Vector3 offset = currentWorldPos - startPoint.position;
            newArm.transform.position += offset;

            // Snapshot endPoint world pos before parenting
            Vector3 endWorldPos = endPoint.position;

            newArm.transform.SetParent(currentParent, worldPositionStays: true);
            newArm.transform.localScale = GetScaleToMatchWorld(currentParent);

            // Spawn new join at endPoint
            GameObject newJoin = Instantiate(joinPrefab, currentParent);
            newJoin.name = $"Join {i + 2}";
            newJoin.transform.position = endWorldPos;
            newJoin.transform.localScale = GetScaleToMatchWorld(currentParent);

            JoinPoint newJoinPoint = newJoin.GetComponent<JoinPoint>();
            joints.Add(newJoinPoint);

            currentParent = newJoin.transform;
            currentWorldPos = endWorldPos;
        }

        // Step 3: Spawn JoinLast
        GameObject joinLast = Instantiate(joinPrefab, currentParent);
        joinLast.name = "JoinLast";
        joinLast.transform.position = currentWorldPos;
        joinLast.transform.localScale = GetScaleToMatchWorld(currentParent);

        // Step 4: Spawn Welder inside JoinLast
        if (welderPrefab != null)
        {
            GameObject welder = Instantiate(welderPrefab, joinLast.transform);
            welder.name = "Welder";
            welder.transform.localPosition = Vector3.zero;
            welder.transform.localScale = GetScaleToMatchWorld(joinLast.transform);

            RewireWelding(welder);
        }
        else
        {
            Debug.LogWarning("welderPrefab is not assigned!");
        }

        JoinPoint joinLastPoint = joinLast.GetComponent<JoinPoint>();
        joints.Add(joinLastPoint);

        // Step 5: Manually assign chain — no need for BuildChainFromHierarchy()
        for (int i = 0; i < joints.Count - 1; i++)
        {
            joints[i].jp_child = joints[i + 1];
        }
        joints[joints.Count - 1].jp_child = null;

        // Step 6: Directly set root and end on ikManager
        ikManager.SetChain(joints[0], joints[joints.Count - 1]);

        ikManager.IsRebuilding = false;

        Debug.Log($"Chain rebuilt with {totalArms} arms. {joints.Count} joints total.");
    }

    private void RewireWelding(GameObject welder)
    {
        Welding weldingComponent = welder.GetComponent<Welding>();

        Debug.Log($"weldingComponent = {weldingComponent}");
        Debug.Log($"playerInput = {playerInput}");

        if (weldingComponent == null || playerInput == null) return;

        InputAction weldingAction = playerInput.actions["Welding"];

        if (weldingAction == null) return;

        weldingComponent.InitWelding(weldingAction);
    }

    private Transform FindDeepChild(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName) return child;
            Transform found = FindDeepChild(child, childName);
            if (found != null) return found;
        }
        return null;
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
    private Vector3 GetScaleToMatchWorld(Transform parent)
    {
        Vector3 parentWorld = parent.lossyScale;
        return new Vector3(
            1f / parentWorld.x,
            1f / parentWorld.y,
            1f / parentWorld.z
        );
    }
}
