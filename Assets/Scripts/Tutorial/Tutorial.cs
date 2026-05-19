using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public void ShowTutorial()
    {
        if (this == null) return;

        gameObject.SetActive(true);
    }
    public void HideTutorial()
    {
        if (this == null) return;

        gameObject.SetActive(false);
    }
}
