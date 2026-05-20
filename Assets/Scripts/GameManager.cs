using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private string gamePlayLevelBGM;
    [SerializeField] private int _level = 1;

    private bool _isTutorialComplete;
    public bool IsTutorialComplete => _isTutorialComplete;

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

    public void OnTutorialComplete()
    {
        _isTutorialComplete = true;
    }

    public void StartGame()
    {
        SceneManager.LoadScene($"Main-GamePlay-Story{_level}");
    }

    public void NextStory()
    {
        _level++;
        SceneManager.LoadScene($"Main-GamePlay-Story{_level}");
    }

    public void NextLevel()
    {
        if (_level >= 4)
        {
            Debug.Log("Congratulations! You've completed all levels!");
            return;
        }

        MusicManager.Instance.PlayMusic(gamePlayLevelBGM);
        SceneManager.LoadScene($"Main-GamePlay-Level{_level}");
    }

    public void ResetGame()
    {
        _level = 1;
    }
}
