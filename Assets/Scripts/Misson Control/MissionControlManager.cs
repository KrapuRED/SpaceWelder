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
public class MissionControlDialoguePerformace
{
    public MissionSuccesType missionSuccesType;
    public DialogueData dialogueData;
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

    [SerializeField] private CaptianRhea captianRhea;
    [SerializeField] private PerfomanceRatingCalculate perfomanceRatingCalculate;
    [SerializeField] private MissionData missionData;
    
    [Header("Mission Control Dialogue")]
    [SerializeField] private List<DialogueData> _missionControlDialoguesFailed =  new();
    [SerializeField] private List<MissionControlDialoguePerformace> _missionControlDialoguesRating = new ();
    [SerializeField] private List<MissionControlData> _missionControlDialogues = new ();
    [SerializeField] private float _InactiveTalking;
    [SerializeField] private bool _isActive = true;
    [SerializeField] private float _lockedTimeout;
    [SerializeField] private MissionControlDialogueBox _dialogueBox;

    private List<MissionSuccesData> _missionSuccesDatas = new();
    private MissionControltype _prevSelectedMissionControltype;
    private EffciencyShipManager _shipManager;
    private DestinationManager _destinationManager;
    [SerializeField] private RobotWelder _currentPlayer;
    private Coroutine _inactiveCoroutine;
   
    private float _lockedTimer;
    private HashSet<MissionControltype> _lockedTypes = new();
    private Dictionary<MissionControltype, float> _typeCooldowns = new();

    private MissionSuccesType _missionSuccesType;
    private bool _missionStarted;
    private bool _missionResultRecorded = false;

    private Queue<DialogueData> _dialogueQueue = new Queue<DialogueData>();
    private bool _isPlayingDialogue = false;

    private Coroutine _delayIdleAnimCoroutine;


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

        if (GameManager.Instance.IsTutorialComplete) 
        {
            TriggerDialogueByType(MissionControltype.Start);

        }

        GlobalEvents.OnMissionGoalUI.Invoke(missionData.ArrivalAt);

        _missionStarted = true;
    }

    #region Event Area

    private void OnEnable()
    {
        GlobalEvents.OnPlayerDeath.AddListener(OnFailedMission);
        GlobalEvents.OnTutorialComplete.AddListener(OnTutorailComplete);
    }

    private void OnDisable()
    {
        RemoveAllEvent();
    }

    private void OnDestroy()
    {
        RemoveAllEvent();
    }

    public void OnReachDestination(float time)
    {
        if (this == null) return;
        CheckDurationMission(time);
    }

    public void OnFailedMission()
    {
        _dialogueQueue.Clear();
        _isPlayingDialogue = false;

        int index = Random.Range(0, _missionControlDialoguesFailed.Count);
        TriggerDialogue(_missionControlDialoguesFailed[index]);
    }

    private void RemoveAllEvent()
    {
        GlobalEvents.OnPlayerDeath.RemoveListener(OnFailedMission);
        GlobalEvents.OnTutorialComplete.AddListener(OnTutorailComplete);

    }

    private void OnTutorailComplete()
    {
        TriggerDialogueByType(MissionControltype.Start);
    }
    #endregion

    private void Update()
    {
        if (!_isActive) return;

        if (!_missionStarted) return;

        if (_destinationManager.IsReachDestinantion) return;

        UpdateCoolDownDialogue();

        CheckEfficienyThreshold();
        CheckDistanceThreshold();
        CheckPlayerHealthThreshold();
    }

    //============== Trigger ================================
    private void TriggerDialogueByType(MissionControltype type)
    {
        if (this == null) return;

        if (captianRhea != null)
            captianRhea.TalkingAnimation();

        MissionControlData mcd = _missionControlDialogues.Find(x => x.MissionControlDataType == type);
        if (mcd == null || mcd.MissionControlDialogueData.Count == 0) return;

        _lockedTypes.Add(type);

        //GlobalEvents.OnMissionControlDialogue.Invoke(mcd.MissionControlDialogueData[0].dialogueData.dialogue);
        _dialogueBox.SetDialogueText(mcd.MissionControlDialogueData[0].dialogueData.dialogue);

        SoundEffectManager.Instance.PlaySoundEffect("Intercom");

        if (_delayIdleAnimCoroutine != null) StopCoroutine(_delayIdleAnimCoroutine);
        _delayIdleAnimCoroutine = StartCoroutine(DelayIdleAnimation());

        if (_inactiveCoroutine != null) StopCoroutine(_inactiveCoroutine);
        _inactiveCoroutine = StartCoroutine(OnInactiveTalking());
    }

    private IEnumerator PlayDialogueQueue()
    {
        _isPlayingDialogue = true;

        while (_dialogueQueue.Count > 0)
        {
            DialogueData data = _dialogueQueue.Dequeue();

            if (captianRhea != null)
                captianRhea.TalkingAnimation();

            if (_dialogueBox == null) yield break;

            _dialogueBox.SetDialogueText(data.dialogue);
            SoundEffectManager.Instance.PlaySoundEffect("Intercom");

            if (_delayIdleAnimCoroutine != null) StopCoroutine(_delayIdleAnimCoroutine);
            _delayIdleAnimCoroutine = StartCoroutine(DelayIdleAnimation());

            // Wait before playing next dialogue
            yield return new WaitForSeconds(_InactiveTalking);
        }

        _isPlayingDialogue = false;
    }

    private void TriggerDialogue(DialogueData data)
    {
        // Add to queue instead of playing immediately
        _dialogueQueue.Enqueue(data);

        // Only start playing if nothing is currently playing
        if (!_isPlayingDialogue)
            StartCoroutine(PlayDialogueQueue());
    }

    //============== Checker Mission ================================
    private void UpdateCoolDownDialogue()
    {
        List<MissionControltype> keys = _typeCooldowns.Keys.ToList();

        foreach (var key in keys)
        {
            _typeCooldowns[key] -= Time.deltaTime;

            if (_typeCooldowns[key] <= 0f)
            {
                _typeCooldowns.Remove(key);
            }
        }
    }

    private void OnTypeFired(MissionControltype type)
    {
        _typeCooldowns[type] = _lockedTimeout;
    }

    //============== Checker Performance Rating ================================

    private void CheckDurationMission(float time)
    {
        if (_missionResultRecorded) return;
        _missionResultRecorded = true;

        var missionSucces = new MissionSuccesData();
        missionSucces.MissionName = missionData.MissionName;
        missionSucces.ArriveAt = time;
        MissionSuccesType rating = MissionSuccesType.OnTime;

        if (time <= missionData.ArrivalAt)
            rating = MissionSuccesType.OnTime;
        else if (time <= missionData.ArrivalAt + missionData.maxDelay)
            rating = MissionSuccesType.Delay;
        else
            rating = MissionSuccesType.Late;

        _missionSuccesType = rating;
        missionSucces.MissionSuccesType = rating;
        _missionSuccesDatas.Add(missionSucces);
        DataPersistenceManager.Instance.SaveGame();
    }

    private void CheckMissionSuccesType(MissionSuccesType type)
    {
        var target = _missionControlDialoguesRating.Find(x => x.missionSuccesType == type);
        if (target == null) return;


        TriggerDialogue(target.dialogueData);
    }

    //============== Checker Mission ================================
    private void CheckEfficienyThreshold()
    {
        if (_shipManager == null) return;
        if (_typeCooldowns.ContainsKey(MissionControltype.Efficiency)) return;

        float efficiency = _shipManager.EfficiencyShip;

        MissionControlData mcd = _missionControlDialogues
            .Find(x => x.MissionControlDataType == MissionControltype.Efficiency);

        if (mcd == null) return;

        // Find the FIRST(highest) threshold that efficiency is at or below
        MissionControlDialogueData target = null;

        if (efficiency >= 100f)
        {
            target = mcd.MissionControlDialogueData.
                FirstOrDefault(x => x.activeAt == 100f);
        }
        else
        {
            target = mcd.MissionControlDialogueData
                .Where(x => efficiency >= x.activeAt && efficiency < 100f)
                .OrderByDescending(x => x.activeAt)
                .FirstOrDefault();
        }

        if (target == null) return;

        TriggerDialogue(target.dialogueData);
        OnTypeFired(MissionControltype.Efficiency);
    }

    private void CheckDistanceThreshold()
    {
        if (_destinationManager == null) return;
        if (_typeCooldowns.ContainsKey(MissionControltype.Distination)) return;

        float totalDistance    = _destinationManager.TotalDistance;
        float currentDistance  = _destinationManager.DistanceToTarget();
        float progressDistance = (1f - (currentDistance / totalDistance)) * 100f;

        MissionControlData mcd = _missionControlDialogues
            .Find(x => x.MissionControlDataType == MissionControltype.Distination);

        if (mcd == null) return;
        foreach (var entry in mcd.MissionControlDialogueData
             .OrderByDescending(x => x.activeAt))
        {
            bool pastThresHold = progressDistance >= entry.activeAt;

            if (pastThresHold)
            {
                TriggerDialogue(entry.dialogueData);
                OnTypeFired(MissionControltype.Distination);
                return;
            }
        }
    }

    private void CheckPlayerHealthThreshold()
    {
        if (_currentPlayer == null) return;
        if (_typeCooldowns.ContainsKey(MissionControltype.Player)) return;

        float playerHealthPercent = _currentPlayer.CurrentHealth;

        MissionControlData mcd = _missionControlDialogues
            .Find(x => x.MissionControlDataType == MissionControltype.Player);
        if (mcd == null) return;

        foreach (var entry in mcd.MissionControlDialogueData
             .OrderBy(x => x.activeAt))
        {
            bool belowThreshold = playerHealthPercent <= entry.activeAt;

            if (belowThreshold)
            {
                TriggerDialogue(entry.dialogueData); 
                OnTypeFired(MissionControltype.Player);
                return;
            }
        }
    }

    IEnumerator DelayIdleAnimation()
    {
        yield return new WaitForSeconds(4f);

        if (captianRhea != null)
            captianRhea.IdleAnimation();
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
        CheckMissionSuccesType(_missionSuccesType);
        GlobalEvents.OnShowPerformancePanel.Invoke(_missionSuccesDatas);
    }
}
