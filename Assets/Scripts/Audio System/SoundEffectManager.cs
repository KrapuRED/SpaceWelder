using UnityEngine;
using System.Collections.Generic;

public class SoundEffectManager : MonoBehaviour
{
    public static SoundEffectManager Instance { get; private set; }

    [SerializeField] private SoundEffectLibrary _library;

    [Header("Sound Effec Audio Source Settings")]
    [SerializeField] private AudioSource _soundSource;
    [SerializeField] private AudioSource _soundSourceLoop;

    private Dictionary<string, AudioSource> _loopSources = new();

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
        if (_loopSources.ContainsKey(groupID))
            return;

        AudioClip clip = _library.GetClipByID(groupID);

        if (clip == null)
        {
            Debug.LogWarning($"Loop clip not found: {groupID}");
            return;
        }

        GameObject loopObj = new GameObject($"Loop_{groupID}");
        loopObj.transform.SetParent(transform);

        AudioSource source = loopObj.AddComponent<AudioSource>();

        source.clip = clip;
        source.loop = true;
        source.Play();

        _loopSources.Add(groupID, source);
    }

    public void StopSoundEffect()
    {
        if (_soundSource.isPlaying)
            _soundSource.Stop();
    }

    public void StopSoundEffectLoop(string groupID)
    {
        if (!_loopSources.TryGetValue(groupID, out AudioSource source))
            return;

        source.Stop();

        Destroy(source.gameObject);

        _loopSources.Remove(groupID);
    }
}
