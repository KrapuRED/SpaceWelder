using UnityEngine;

public class ButtonNextMission : MonoBehaviour
{
    public void OnClickButtonUpgrade()
    {
        GlobalEvents.OnHidePerformacnePanel.Invoke();

        DataPersistenceManager.Instance.SaveGame();
        GameManager.Instance.NextStory();
    }
}
