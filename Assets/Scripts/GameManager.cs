using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private int _level;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void NextLevel()
    {
        _level++;
        Debug.Log($"Change Scene to {_level}");
        SceneManager.LoadScene($"Main-GamePlay-Level{_level}");
    }

    public void ResetGame()
    {
        _level = 1;
    }
}
