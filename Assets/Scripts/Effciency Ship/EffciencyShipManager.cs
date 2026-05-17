using UnityEngine;

public class EffciencyShipManager : MonoBehaviour
{
    public static EffciencyShipManager Instance;

    [Range(0, 100)]
    [SerializeField] private float effciencyShip;
    [SerializeField] private float _effciencyShipLerpSpeed;
    [SerializeField] private float _perHullDamageEfficiencyDrop;
    [SerializeField] private int _totalPossibleHullBreach;

    ManagerHullShip _managerHullShip;
    private float _targetEffciency;

    public float EffciencyShip => effciencyShip;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        _managerHullShip = ManagerHullShip.Insantce;

        _totalPossibleHullBreach = _managerHullShip.PossibleHullDamages;
    }

    private void Update()
    {
        _targetEffciency = CalculateEffciencyShip();

        effciencyShip = Mathf.Lerp(effciencyShip, _targetEffciency, _effciencyShipLerpSpeed * Time.deltaTime);

        GlobalEvents.OnShipEffciencyUI.Invoke(effciencyShip/100f);
    }

    private float CalculateEffciencyShip()
    {
        float efficiency = 100;

        if (_totalPossibleHullBreach == 0) return efficiency;

        int activeBreaches = _managerHullShip.ActiveHullBreachs;

        float totalDrop = activeBreaches * _perHullDamageEfficiencyDrop;
        efficiency =  Mathf.Clamp(100f - totalDrop, 0f, 100f);

        return efficiency;
    }
}
