using UnityEngine;

public class ButtonNextMission : MonoBehaviour
{
    public void OnClickButtonUpgrade()
    {
        SoundEffectManager.Instance.StopSoundEffectLoop("AlertEfficiency");
        GlobalEvents.OnHidePerformacnePanel.Invoke();

        DataPersistenceManager.Instance.SaveGame();
        GameManager.Instance.NextStory();
    }
}
