using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public string musicTrackName;
    void Start()
    {
        MusicManager.Instance.PlayMusic(musicTrackName);
    }

    public void StartGame()
    {
        Debug.Log("Start Game");
        GameManager.Instance.StartGame();
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
