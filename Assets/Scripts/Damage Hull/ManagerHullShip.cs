using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;

[System.Serializable]
public class PhaseData
{
    public string phaseName;
    public string phaseID;
    public float activeAt;
    public float minSpawnRate;
    public float maxSpawnRate;
}


[System.Serializable]
public class HullBreachData
{
    public string hullID;
    public bool IsHullBreach;
}

public class ManagerHullShip : MonoBehaviour
{
    public static ManagerHullShip Insantce;

    [Header("Damage Hull Breach Control")]
    [SerializeField] private List<DamageHull> damageHulls = new ();
    [SerializeField] private List<PhaseData> phaseDatas = new ();
    [SerializeField] private List<HullBreachData> hullBreachDatas = new ();
    [SerializeField] private List<Sprite> _aviableHullBreachSprites = new();
    [SerializeField] private int _limitHullBreach;
    private int _activePhaseIndex;

    [SerializeField] private PhaseData _selectedPhaseData;
    private Coroutine _activeManagerDamageHull;
    private bool _reachDestination;

    public int PossibleHullDamages => damageHulls.Count;
    public int ActiveHullBreachs => hullBreachDatas.Count;

    private HashSet<string> _activePhases = new HashSet<string>();

    private void Awake()
    {
        if (Insantce == null)
            Insantce = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        if (phaseDatas.Count <= 0)
        {
            Debug.LogError("[ManagerHullShip] phaseDatas Dont have any Phase Data!");
            return;
        }
        _selectedPhaseData = phaseDatas[0];
        OnStartHullBreach(_activePhaseIndex);
    }

    private void OnEnable()
    {
        GlobalEvents.OnHullBeenReapir.AddListener(OnHullBeenReapir);
        GlobalEvents.OnReachDestination.AddListener(ReachDestination);
    }

    private void OnDisable()
    {
        Unsubscripe();
    }

    private void OnDestroy()
    {
        Unsubscripe();
    }

    private void ReachDestination()
    {
        _reachDestination = true;
    }

    public void CheckPhase(float time)
    {
        foreach (var phase in phaseDatas)
        {
            if (phase.activeAt <= time && !_activePhases.Contains(phase.phaseID))
            {
                _activePhases.Add(phase.phaseID);
                OnChangePhase(phase);
            }
        }
    }

    private HullBreachData FindHullBreachDataByID(string hullID)
    {
        HullBreachData data = null;

        foreach (var hullBreach in hullBreachDatas)
        {
            if (hullBreach.hullID == hullID)
                data = hullBreach;
        }

        return data;
    }

    private Sprite GetRandomHullBreachSprite()
    {
        int index = Random.Range(0, _aviableHullBreachSprites.Count);

        return _aviableHullBreachSprites[index];
    }

    private void OnStartHullBreach(int phaseIndex)
    {
        _activePhaseIndex = phaseIndex;
        _selectedPhaseData = phaseDatas[phaseIndex];

        _activeManagerDamageHull = StartCoroutine(OnDelayHullBreach());
    }

    private void OnChangePhase(PhaseData phase)
    {
        _selectedPhaseData = phase;
        Debug.Log($"Phase change to {phase.phaseName}");

        if (_activeManagerDamageHull != null) StopCoroutine(_activeManagerDamageHull);
        _activeManagerDamageHull = StartCoroutine(OnDelayHullBreach());
    }

    private bool CheckHullBreachData(string hullID)
    {
        bool isFound = false;

        foreach (var hullBreach in hullBreachDatas)
        {
            if (hullBreach.hullID == hullID)
                isFound = true;
        }

        return isFound;
    }

    private void OnHullBeenReapir(string hullID)
    {
        if (!CheckHullBreachData(hullID))
        {
            Debug.Log($"There are no Hull breach in {hullID}");
            return;
        }

        var removeData = FindHullBreachDataByID(hullID);

        hullBreachDatas.Remove(removeData);
    }

    private void OnHullBreach()
    {
        if (_reachDestination) return;

        if (damageHulls.Count <= 0)
        {
            Debug.Log($"Damage Hull in {gameObject.name} is empty");
            return;
        }

        if (hullBreachDatas.Count >= _limitHullBreach)
        {
            _activeManagerDamageHull = StartCoroutine(OnDelayHullBreach());
            return;
        }

        var availableHulls = damageHulls.FindAll(h => !CheckHullBreachData(h.HullID));
        if (availableHulls.Count <= 0) return;

        int index = Random.Range(0, availableHulls.Count);
        var possibel = availableHulls[index];

        Sprite sprite = GetRandomHullBreachSprite();

        if (sprite == null)
            return;

        possibel.OnHullBreach(sprite);

        HullBreachData newData = new HullBreachData
        {
            hullID = possibel.HullID,
            IsHullBreach = possibel.IsHullBreach
        };

        hullBreachDatas.Add(newData);

        _activeManagerDamageHull = StartCoroutine(OnDelayHullBreach());
    }

    private float GetRandomSpawnRate()
    {
        float time = Random.Range(_selectedPhaseData.minSpawnRate, _selectedPhaseData.maxSpawnRate);
        return time;
    }

    private IEnumerator OnDelayHullBreach()
    {
        yield return new WaitForSeconds(GetRandomSpawnRate());
        OnHullBreach();
    }

    private void Unsubscripe()
    {
        GlobalEvents.OnReachDestination.RemoveListener(ReachDestination);
        GlobalEvents.OnHullBeenReapir.RemoveListener(OnHullBeenReapir);
    }
}
