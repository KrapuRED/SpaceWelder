using UnityEngine;

public class FABRIKSolver : MonoBehaviour
{
    #region Configuration Structs and Enums
    [System.Serializable]
    public struct JointLimit
    {
        [Tooltip("Label — purely for readability in the Inspector.")]
        public string label;

        [Range(-180f, 0f)] public float minAngle;   
        [Range(0f, 180f)] public float maxAngle;   

        [Tooltip("Which axis this joint rotates around.")]
        public RotationAxis axis;
    }

    public enum RotationAxis
    {
        LocalX, LocalY, LocalZ,
        WorldUp, WorldRight, WorldForward
    }

    [Header("Bone Chain")]
    [Tooltip("Drag bones in order from root to tip.")]
    public Transform[] bones;

    [Header("IK Target")]
    [Tooltip("Empty GO that CursorIKTarget moves. This is what the arm chases.")]
    public Transform ikTarget;

    [Header("Solver")]
    [Range(1, 30)] public int iterations;
    public float tolerance;
    [Range(1, 30)] public float smoothSpeed;

    [Header("Joint Limits — one entry per bone")]
    public JointLimit[] jointLimits;

    [Header("Pole Target  (keeps elbow from flipping)")]
    [Tooltip("Place to the right-side of the arm, same height as mid-bone.")]
    public Transform poleTarget;
    [Range(0f, 1f)] public float poleWeight = 0.7f;

    private float[] _lenghts;
    private float _totalLength;
    private Vector3[] _postion;

    #endregion

    #region Lifecycle Methods

    private void Awake() => Initialized();

    private void Initialized()
    {
        if (bones == null || bones.Length < 2)
        {
            Debug.LogError("FABRIKSolver requires at least 2 bones to function.", this);
            enabled = false;
            return;
        }

        int totalLengthBones = bones.Length;
        _lenghts = new float[totalLengthBones - 1];
        _postion = new Vector3[totalLengthBones];

        for (int i = 0; i < totalLengthBones - 1; i++)
        {
            _lenghts[i] = Vector3.Distance(bones[i].position, bones[i + 1].position);
            _totalLength += _lenghts[i];
        }

        if (jointLimits == null || jointLimits.Length != totalLengthBones)
        {
            DefaultBoomArmLimits(totalLengthBones);
        }
    }

    void DefaultBoomArmLimits(int totalLengthBones)
    {
        jointLimits = new JointLimit[totalLengthBones];

        jointLimits[0] = new JointLimit
        {
            label = "Root (yaw)",
            minAngle = -180f,
            maxAngle = 180f,
            axis = RotationAxis.WorldUp
        };

        if (totalLengthBones > 1)
        {
            jointLimits[1] = new JointLimit
            {
                label = "Boom",
                minAngle = -70f,
                maxAngle = 10f,
                axis = RotationAxis.LocalX
            };
        }

        if (totalLengthBones > 2)
        {
            jointLimits[2] = new JointLimit
            {
                label = "Arm/Stick",
                minAngle = -10f,
                maxAngle = 130f,
                axis = RotationAxis.LocalX
            };
        }

        if (totalLengthBones > 3)
        {
            jointLimits[3] = new JointLimit
            {
                label = "Tip/Bucket",
                minAngle = -90f,
                maxAngle = 90f,
                axis = RotationAxis.LocalX
            };
        }

        for (int i = 4; i < totalLengthBones; i++)
            jointLimits[i] = new JointLimit
            {
                label = "Bone " + i,
                minAngle = -45f,
                maxAngle = 45f,
                axis = RotationAxis.LocalX
            };
    }

    private void LateUpdate()
    {
        if (ikTarget == null)
        {
            Debug.LogWarning("FABRIKSolver requires an IK Target to function.", this  );
            return;
        }
        Solve(ikTarget.position, ikTarget.rotation);
    }

    #endregion

    public float GetTotalLength() => _totalLength;

    public void Solve(Vector3 targetPos, Quaternion targetRot)
    {
        for (int i = 0; i < bones.Length; i++)
            _postion[i] = bones[i].position;

        RunFABRIK(targetPos);

        if (poleTarget != null)
            ApplyPole();

        ApplyToBoneSmooth(targetRot);
    }

    private void RunFABRIK(Vector3 target)
    {
        int totalBones = bones.Length;
        Vector3 rootPos = _postion[0];

        if (Vector3.Distance(rootPos, target) >= _totalLength)
        {
            Vector3 distance = (target - rootPos).normalized;
            for (int i = 1; i < totalBones; i++)
                _postion[i] = _postion[i - 1] + distance * _lenghts[i - 1];
            return;
        }

        for (int iter = 0; iter < iterations; iter++)
        {
            _postion[totalBones - 1] = target;

            for (int i = totalBones - 2; i >= 0; i--)
            {
                Vector3 distance = (_postion[i] - _postion[i + 1]).normalized;
                _postion[i] = _postion[i + 1] + distance * _lenghts[i];
            }

            _postion[0] = rootPos;

            for (int i = 1; i < totalBones; i++)
            {
                Vector3 distance = (_postion[i] - _postion[i - 1]).normalized;
                _postion[i] = _postion[i - 1] + distance * _lenghts[i - 1];
                ClampJoint(i);
            }

            if (Vector3.Distance(_postion[totalBones - 1], target) < tolerance)
                break;
        }
    }

    private void ClampJoint(int i)
    {
        if (i <= 0 || i >= bones.Length - 1)
            return;

        Vector3 incoming = (_postion[i] - _postion[i-1]).normalized;
        Vector3 outgoing = (_postion[i + 1] - _postion[i]).normalized;

        Vector3 axis = GetAxis(i);
        float angle = Vector3.SignedAngle(incoming, outgoing, axis);
        float clampedAngle = Mathf.Clamp(angle, 
                                         jointLimits[i].minAngle, 
                                         jointLimits[i].maxAngle);

        if (Mathf.Abs(angle - clampedAngle) > 0.01f)
        {
            Quaternion fix = Quaternion.AngleAxis(clampedAngle - angle, axis);
            _postion[i+1] = _postion[i] + fix * (outgoing * _lenghts[i]);
        }
    }

    Vector3 GetAxis(int i)
    {
        if (i >= jointLimits.Length) return bones[i].right;
        return jointLimits[i].axis switch
        {
            RotationAxis.LocalX => bones[i].right,
            RotationAxis.LocalY => bones[i].up,
            RotationAxis.LocalZ => bones[i].forward,
            RotationAxis.WorldUp => Vector3.up,
            RotationAxis.WorldRight => Vector3.right,
            RotationAxis.WorldForward => Vector3.forward,
            _ => bones[i].right,
        };
    }

    void ApplyPole()
    {
        if (bones.Length < 3) return;

        int mid = Mathf.Max(1, bones.Length / 2);
        Vector3 root = _postion[0];
        Vector3 tip = _postion[^1];
        Vector3 chainExis = (tip - root).normalized;

        Vector3 midFlat  = Vector3.ProjectOnPlane(_postion[mid] - root, chainExis);
        Vector3 poleFlat = Vector3.ProjectOnPlane(poleTarget.position - root, chainExis);

        if (midFlat.sqrMagnitude < 0.0001f || poleFlat.sqrMagnitude < 0.0001f)
            return;

        float angle = Vector3.SignedAngle(midFlat, poleFlat, chainExis);
        Quaternion rot = Quaternion.AngleAxis(angle * poleWeight, chainExis);

        for (int i = 1; i < bones.Length; i++)
        {
            _postion[i] = root + rot * (_postion[i] - root);
        }
    }

    void ApplyToBoneSmooth(Quaternion tipRotation)
    {
        float time = Time.deltaTime * smoothSpeed;

        for (int i = 0; i < bones.Length - 1; i++)
        {
           Vector3 curDirection = bones[i + 1].position - bones[i].position;
           Vector3 solDirection = _postion[i + 1] - _postion[i];

            Quaternion delta = Quaternion.FromToRotation(curDirection, solDirection);
            bones[i].rotation = Quaternion.Slerp(bones[i].rotation,
                                                 delta * bones[i].rotation, time);
        }

        Vector3 dir = ikTarget.position - bones[^1].position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion targetRot = Quaternion.Euler(0, 0, angle);

        bones[^1].rotation = Quaternion.Slerp(bones[^1].rotation, targetRot, time);
    }

    void OnDrawGizmos()
    {
        if (bones == null) return;

        // Bone chain
        Gizmos.color = Color.cyan;
        for (int i = 0; i < bones.Length - 1; i++)
            if (bones[i] && bones[i + 1])
                Gizmos.DrawLine(bones[i].position, bones[i + 1].position);

        // Joint dots
        Gizmos.color = Color.white;
        foreach (var b in bones)
            if (b) Gizmos.DrawWireSphere(b.position, 0.025f);

        // IK target
        if (ikTarget)
        { Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(ikTarget.position, 0.05f); }

        // Pole target
        if (poleTarget)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(poleTarget.position, 0.04f);
            if (bones.Length > 1 && bones[bones.Length / 2])
                Gizmos.DrawLine(bones[bones.Length / 2].position, poleTarget.position);
        }

        // Reach sphere
        if (Application.isPlaying && bones[0])
        {
            Gizmos.color = new Color(1f, 1f, 0f, 0.1f);
            Gizmos.DrawWireSphere(bones[0].position, _totalLength);
        }
    }
}
