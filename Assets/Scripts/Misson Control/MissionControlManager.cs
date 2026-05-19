using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine.Rendering;

[System.Serializable]
public enum MissionControltype
{
    Start,
    Efficiency,
    Distination,
    Player
}

[System.Serializable]
public enum MissionSuccesType
{
    OnTime,
    Delay,
    Late
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
    public float ArrivalAt;
    public float maxDelay; 
}

public class MissionControlManager : MonoBehaviour, IDataPersistence
{
    public static MissionControlManager Instance;

    [SerializeField] private MissionData missionData;
    
    [Header("Mission Control Dialogue")]
    [SerializeField] private List<MissionControlData> _missionControlDialogues = new ();
    [SerializeField] private float _InactiveTalking;
    [SerializeField] private bool _isActive = true;
    [SerializeField] private float _lockedTimeout;

    private MissionControltype _prevSelectedMissionControltype;
    private EffciencyShipManager _shipManager;
    private DestinationManager _destinationManager;
    [SerializeField] private RobotWelder _currentPlayer;
    private Coroutine _inactiveCoroutine;
    private float _lockedTimer;

    private List<MissionSuccesData> _missionSuccesDatas = new();
    private HashSet<int> _firedPlayerThresholds = new();
    private HashSet<int> _firedEfficiencyThresholds = new();
    private HashSet<int> _firedDistanceThresholds = new();
    private HashSet<MissionControltype> _lockedTypes = new();

    private bool _missionStarted;
    private bool _missionResultRecorded = false;

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
        GlobalEvents.OnMissionGoalUI.Invoke(missionData.ArrivalAt);

        _missionStarted = true;
    }

    #region Event Area

    public void OnReachDestination(float time)
    {
        if (this == null) return;
        CheckDurationMission(time);
    }
    #endregion

    private void Update()
    {
        if (!_isActive) return;

        if (!_missionStarted) return;

        if (_lockedTypes.Count > 0)
        {
            _lockedTimer += Time.deltaTime;
            if (_lockedTimer >= _lockedTimeout)
            {
                _lockedTimer = 0f;
                _lockedTypes.Clear();
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

    private void CheckDurationMission(float time)
    {
        Debug.Log(time);
        if (_missionResultRecorded) return;
        _missionResultRecorded = true;

        var missionSucces = new MissionSuccesData();
        missionSucces.MissionName = missionData.MissionName;
        missionSucces.ArriveAt = time;

        if (time <= missionData.ArrivalAt)
            missionSucces.MissionSuccesType = MissionSuccesType.OnTime;
        else if (time <= missionData.ArrivalAt + missionData.maxDelay) 
            missionSucces.MissionSuccesType = MissionSuccesType.Delay;
        else
            missionSucces.MissionSuccesType = MissionSuccesType.Late;

        _missionSuccesDatas.Add(missionSucces);
        DataPersistenceManager.Instance.SaveGame();
    }

    private void OnTypeFired(MissionControltype type)
    {
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
        float progressDistance = (1f - (currentDistance / totalDistance)) * 100f;

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

    IEnumerator OnInactiveTalking()
    {
        _isActive = false;
        yield return new WaitForSeconds(_InactiveTalking);
        _isActive = true;
    }

    public void LoadData(GameData data)
    {
        _missionSuccesDatas = new List<MissionSuccesData>(data.missionSuccesDatas);
    }

    public void SaveData(ref GameData data)
    {
        data.missionSuccesDatas = new List<MissionSuccesData>(_missionSuccesDatas);
    }

    public void OnShowPerformace() 
    {

        GlobalEvents.OnShowPerformancePanel.Invoke(_missionSuccesDatas);
    }
}
