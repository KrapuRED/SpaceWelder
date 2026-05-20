using UnityEngine;

public class SoundEffectManager : MonoBehaviour
{
    public static SoundEffectManager Instance { get; private set; }

    [SerializeField] private SoundEffectLibrary _library;

    [Header("Sound Effec Audio Source Settings")]
    [SerializeField] private AudioSource _soundSource;
    [SerializeField] private AudioSource _soundSourceLoop;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void PlaySoundEffect(string groupID)
    {
        AudioClip clip = _library.GetClipByID(groupID);
        if (clip != null)
            _soundSource.PlayOneShot(clip);
        else
            Debug.LogWarning($"[SoundEffectManager] Clip not found: {groupID}");
    }

    public void PlaySoundEffectLoop(string groupID)
    {
        if (_soundSourceLoop.isPlaying) return;

        Debug.Log($"[SoundEffectManager] PlaySoundEffectLoop by : {groupID}");

        AudioClip clip = _library.GetClipByID(groupID);
        if (clip != null)
        {
            _soundSourceLoop.clip = clip;
            _soundSourceLoop.loop = true; 
            _soundSourceLoop.Play();
        }
        else
        {
            Debug.LogWarning($"[SoundEffectManager] Loop clip not found: {groupID}");
        }
    }

    public void StopSoundEffect()
    {
        if (_soundSource.isPlaying)
            _soundSource.Stop();
    }

    public void StopSoundEffectLoop()
    {
        Debug.Log($"[Stop] isPlaying: {_soundSourceLoop.isPlaying}, clip: {_soundSourceLoop.clip}");

        if (_soundSourceLoop == null) return;

        _soundSourceLoop.Stop();
        _soundSourceLoop.clip = null;
    }
}
