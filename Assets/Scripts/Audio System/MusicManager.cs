using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [SerializeField] private MusicLibrary _library;

    [Header("Music Audio Source Settings")]
    [SerializeField] private AudioSource _musicSource;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void PlayMusic(string trackName, float fadeDuration = 0.5f)
    {
        StartCoroutine(AnimateMusicCrossFade(_library.GetTrack(trackName), fadeDuration));
    }

    IEnumerator AnimateMusicCrossFade(AudioClip audioClip, float duration)
    {
        float percent = 0f;

        while (percent < 1f)
        {
            percent += Time.deltaTime * 1 / duration;
            _musicSource.volume = Mathf.Lerp(1f, 0, percent);
            yield return null;
        }

        _musicSource.clip = audioClip;
        _musicSource.Play();

        percent = 0f;
        while (percent < 1f)
        {
            percent += Time.deltaTime * 1 / duration;
            _musicSource.volume = Mathf.Lerp(0, 1f, percent);
            yield return null;
        }
    }
}
