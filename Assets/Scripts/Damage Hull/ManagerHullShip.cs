using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;

[System.Serializable]
public class PhaseData
{
    public string phaseName;
    public string phaseID;
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
    [SerializeField] private int _activePhaseIndex;

    private PhaseData _selectedPhaseData;
    private Coroutine _activeManagerDamageHull;

    public int PossibleHullDamages => damageHulls.Count;
    public int ActiveHullBreachs => hullBreachDatas.Count;

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

        OnStartHullBreach(_activePhaseIndex);
    }

    private void OnEnable()
    {
        GlobalEvents.OnHullBeenReapir.AddListener(OnHullBeenReapir);
    }

    private void OnDisable()
    {
        Unsubscripe();
    }

    private void OnDestroy()
    {
        Unsubscripe();

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

    private void OnStartHullBreach(int phaseIndex)
    {
        _activePhaseIndex = phaseIndex;
        _selectedPhaseData = phaseDatas[phaseIndex];

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
        int index = Random.Range(0, damageHulls.Count);
        var possibel = damageHulls[index];

        if (CheckHullBreachData(possibel.HullID))
        {
            _activeManagerDamageHull = StartCoroutine(OnDelayHullBreach());
            return;
        }

        possibel.OnHullBreach();

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
        GlobalEvents.OnHullBeenReapir.RemoveListener(OnHullBeenReapir);
    }
}
