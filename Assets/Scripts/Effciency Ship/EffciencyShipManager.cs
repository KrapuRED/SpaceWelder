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
    private bool _isAlertPlaying = false;
    public float EfficiencyShip => effciencyShip;

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

        if (effciencyShip <= 60 && !_isAlertPlaying)
        {
            _isAlertPlaying = true;
            SoundEffectManager.Instance.PlaySoundEffectLoop("AlertEfficiency");
        }
        else if (effciencyShip > 60 && _isAlertPlaying)
        {
            _isAlertPlaying = false;
            SoundEffectManager.Instance.StopSoundEffectLoop("AlertEfficiency");
        }

        GlobalEvents.OnShipEffciencyUI.Invoke(effciencyShip/100f);
    }

    private float CalculateEffciencyShip()
    {
        float efficiency = 100;

        if (_totalPossibleHullBreach == 0) return 100;

        int activeBreaches = _managerHullShip.ActiveHullBreachs;

        float totalDrop = activeBreaches * _perHullDamageEfficiencyDrop;
        efficiency =  Mathf.Clamp(100f - totalDrop, 0f, 100f);

        return efficiency;
    }
}
