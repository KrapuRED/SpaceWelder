using UnityEngine;

public class ButtonRestart : MonoBehaviour
{
    public void OnClickButton()
    {

        GameManager.Instance.PlayLevel();
    }
}
