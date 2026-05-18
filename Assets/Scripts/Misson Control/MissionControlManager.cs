using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

[System.Serializable]
public enum MissionControltype
{
    Start,
    Efficiency,
    Distination,
    Player
}

[System.Serializable]
public class MissionControlDialogueData
{
    public int activeAt;
    public DialogueData dialogueData;
}

[System.Serializable]
public class MissionControlData
{
    public string NameMissionControlData;
    public MissionControltype MissionControlDataType;
    
    public List<MissionControlDialogueData> MissionControlDialogueData = new();
}

[System.Serializable]
public class MissionData
{
    public string MissionName;
    public float ArvialAt;
}

public class MissionControlManager : MonoBehaviour
{
    public static MissionControlManager Instance;

    [SerializeField] private List<MissionControlData> _missionControlDialogues = new ();
    [SerializeField] private float _InactiveTalking;
    [SerializeField] private bool _isActive = true;

    private MissionControltype _prevSelectedMissionControltype;
    [SerializeField] private float _lockedTimeout;
    [SerializeField] private EffciencyShipManager _shipManager;
    [SerializeField] private DestinationManager _destinationManager;
    [SerializeField] private RobotWelder _currentPlayer;
    private Coroutine _inactiveCoroutine;
    private float _lockedTimer;

    private HashSet<int> _firedPlayerThresholds = new();
    private HashSet<int> _firedEfficiencyThresholds = new();
    private HashSet<int> _firedDistanceThresholds = new();

    private HashSet<MissionControltype> _lockedTypes = new();
    private MissionControltype _lastFiredType = MissionControltype.Start;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        _shipManager = EffciencyShipManager.Instance;
        _destinationManager = DestinationManager.Instance;

        TriggerDialogueByType(MissionControltype.Start);
    }

    private void Update()
    {
        if (!_isActive) return;

        if (!_lockedTypes.Contains(MissionControltype.Start)) return;

        if (_lockedTypes.Count > 0)
        {
            _lockedTimer += Time.deltaTime;
            if (_lockedTimer >= _lockedTimeout)
            {
                _lockedTimer = 0f;
                _lockedTypes.Clear(); // force unlock all
            }
        }
        else
        {
            _lockedTimer = 0f;
        }

        CheckEfficienyThreshold();
        CheckDistanceThreshold();
        CheckPlayerHealthThreshold();
    }

    private void OnTypeFired(MissionControltype type)
    {
        _lastFiredType = type;

        _lockedTypes.Add(type);

        foreach (MissionControltype t in System.Enum.GetValues(typeof(MissionControltype)))
        {
            if (t == type || t == MissionControltype.Start) continue;
            _lockedTypes.Remove(t);
        }
    }

    //============== Trigger ================================
    private void TriggerDialogueByType(MissionControltype type)
    {
        MissionControlData mcd = _missionControlDialogues.Find(x => x.MissionControlDataType == type);
        if (mcd == null || mcd.MissionControlDialogueData.Count == 0) return;

        _lockedTypes.Add(type);
        GlobalEvents.OnMissionControlDialogue.Invoke(mcd.MissionControlDialogueData[0].dialogueData.dialogue);

        if (_inactiveCoroutine != null) StopCoroutine(_inactiveCoroutine);
        _inactiveCoroutine = StartCoroutine(OnInactiveTalking());
    }

    private void TriggerDialogue(DialogueData data)
    {
        Debug.Log($"[MissionControl]: {data.dialogue}");
        GlobalEvents.OnMissionControlDialogue.Invoke(data.dialogue);
        if (_inactiveCoroutine != null) StopCoroutine(_inactiveCoroutine);
        _inactiveCoroutine = StartCoroutine(OnInactiveTalking());
    }

    //============== Checker ================================
    private void CheckEfficienyThreshold()
    {
        if (_shipManager == null) return;
        if (_lockedTypes.Contains(MissionControltype.Efficiency)) return;

        float efficiency = _shipManager.EfficiencyShip;

        MissionControlData mcd = _missionControlDialogues
            .Find(x => x.MissionControlDataType == MissionControltype.Efficiency);

        if (mcd == null) return;

        var sorted = mcd.MissionControlDialogueData
        .OrderByDescending(x => x.activeAt)
        .ToList();

        // Find the FIRST(highest) threshold that efficiency is at or below
        var target = mcd.MissionControlDialogueData
       .Where(x => x.activeAt >= efficiency &&
                   !_firedEfficiencyThresholds.Contains(x.activeAt))
       .OrderBy(x => x.activeAt)
       .FirstOrDefault();

        if (target == null) return;

        _firedEfficiencyThresholds.Add(target.activeAt);
        TriggerDialogue(target.dialogueData);
        OnTypeFired(MissionControltype.Efficiency);

        if (_firedEfficiencyThresholds.Count >= mcd.MissionControlDialogueData.Count)
            _firedEfficiencyThresholds.Clear();
    }

    private void CheckDistanceThreshold()
    {
        if (_destinationManager == null) return;
        if (_lockedTypes.Contains(MissionControltype.Distination)) return;

        float totalDistance    = _destinationManager.TotalDistance;
        float currentDistance  = _destinationManager.DistanceToTarget();
        float progressDistance = (1f - (currentDistance - totalDistance)) * 100f;

        MissionControlData mcd = _missionControlDialogues
            .Find(x => x.MissionControlDataType == MissionControltype.Distination);

        if (mcd == null) return;
        foreach (var entry in mcd.MissionControlDialogueData)
        {
            bool pastThresHold = progressDistance >= entry.activeAt;
            bool isAlreadyFire = _firedDistanceThresholds.Contains(entry.activeAt);

            if (pastThresHold && !isAlreadyFire)
            {
                _firedDistanceThresholds.Add(entry.activeAt);
                TriggerDialogue(entry.dialogueData);
                OnTypeFired(MissionControltype.Distination);
                return;
            }
        }

        if (_firedDistanceThresholds.Count >= mcd.MissionControlDialogueData.Count)
            _firedDistanceThresholds.Clear();
    }

    private void CheckPlayerHealthThreshold()
    {
        if (_currentPlayer == null) return;
        if (_lockedTypes.Contains(MissionControltype.Player)) return;

        float playerHealthPercent = _currentPlayer.CurrentHealth;

        MissionControlData mcd = _missionControlDialogues
            .Find(x => x.MissionControlDataType == MissionControltype.Player);
        if (mcd == null) return;

        foreach (var entry in mcd.MissionControlDialogueData)
        {
            bool belowThreshold = playerHealthPercent <= entry.activeAt;
            bool alreadyFired = _firedPlayerThresholds.Contains(entry.activeAt);

            if (belowThreshold && !alreadyFired)
            {
                _firedPlayerThresholds.Add(entry.activeAt);
                TriggerDialogue(entry.dialogueData); 
                OnTypeFired(MissionControltype.Player);
                return;
            }
        }

        if (_firedPlayerThresholds.Count >= mcd.MissionControlDialogueData.Count)
            _firedPlayerThresholds.Clear();
    }

    private void ClearLockedTypes()
    {
        foreach (MissionControltype t in System.Enum.GetValues(typeof(MissionControltype)))
        {
            if (t == MissionControltype.Start) continue;
            _lockedTypes.Remove(t);
        }
    }

    IEnumerator OnInactiveTalking()
    {
        _isActive = false;
        yield return new WaitForSeconds(_InactiveTalking);
        _isActive = true;
    }
}
