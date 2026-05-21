using UnityEngine;

public class ButtonMainMenu : MonoBehaviour
{
    public void OnClickButton()
    {
        SoundEffectManager.Instance.PlaySoundEffect("ClickBottom");
        SoundEffectManager.Instance.StopAllSoundEFfectLoop();

        GameManager.Instance.BackToMainMenu();
    }
}
