using UnityEngine;

[System.Serializable]
public struct SoundEffect
{
    public string groupID;
    public AudioClip[] clips;
}

public class SoundEffectLibrary : MonoBehaviour
{
    public SoundEffect[] soundEffects;

    public AudioClip GetClipByID(string groupID)
    {
        foreach (var effect in soundEffects)
        {
            if (effect.groupID == groupID)
            {
                if (effect.clips.Length > 0)
                {
                    int index = Random.Range(0, effect.clips.Length);
                    return effect.clips[index];
                }
                else
                {
                    Debug.LogWarning($"Sound effect group '{groupID}' has no clips!");
                    return null;
                }
            }
        }
        Debug.LogWarning($"Sound effect group '{groupID}' not found!");
        return null;
    }
}
