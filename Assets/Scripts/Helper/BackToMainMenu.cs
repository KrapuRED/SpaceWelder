using UnityEngine;

public class BackToMainMenu : MonoBehaviour
{
    public void OnCreditDone()
    {
        Debug.Log("Congratulations! You've completed the game!");
        GameManager.Instance.BackToMainMenu();
    }
}
