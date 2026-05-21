using UnityEngine;
using FirstGearGames.SmoothCameraShaker;

public class ExplosionVFX : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private string _explosionTrigger = "Explode";

    public ShakeData shakeData;

    private static readonly int ExplodeTrigger = Animator.StringToHash("Explode");

    private void Awake()
    {
        if (_animator == null)
            _animator = GetComponent<Animator>();
    }

    public void PlayExplosion()
    {
        gameObject.SetActive(true);
        CameraShakerHandler.Shake(shakeData);
        _animator.SetTrigger(ExplodeTrigger);
    }

    // Call this from Animation Event at the last frame
    public void OnExplosionFinished()
    {
        gameObject.SetActive(false);
    }
}
