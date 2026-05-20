using UnityEngine;

[System.Serializable]
public struct MusicTrack
{
    public string Name;
    public AudioClip Clip;
}

public class MusicLibrary : MonoBehaviour
{
    public MusicTrack[] Tracks;

    public AudioClip GetTrack(string name)
    {
        foreach (var track in Tracks)
        {
            if (track.Name == name)
                return track.Clip;
        }
        Debug.LogWarning($"Music track '{name}' not found!");
        return null;
    }
}
