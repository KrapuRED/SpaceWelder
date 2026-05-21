using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private string gamePlayLevelBGM;
    [SerializeField] private int _level = 1;

    [SerializeField] private bool _isTutorialComplete;
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

    private void Start()
    {
        DataPersistenceManager.Instance.NewGame();
    }

    public void StartGame()
    {
        SceneTransitionManager.Instance.LoadScene($"Main-GamePlay-Story{_level}", "CrossFade");
        DataPersistenceManager.Instance.NewGame();
    }

    public void NextStory()
    {
        _level++;
        SceneTransitionManager.Instance.LoadScene($"Main-GamePlay-Story{_level}", "CrossFade");
    }

    public void PlayLevel()
    {
        if (_level >= 4)
        {
            Debug.Log("Congratulations! You've completed all levels!");
            SceneTransitionManager.Instance.LoadScene($"Credit", "CrossFade");

            return;
        }

        MusicManager.Instance.PlayMusic(gamePlayLevelBGM);

        if (_level <= 1)
        {
            SceneManager.LoadScene($"Main-GamePlay-Level{_level}");
        }
        else
            SceneTransitionManager.Instance.LoadScene($"Main-GamePlay-Level{_level}", "CrossFade");
    }

    public void BackToMainMenu()
    {
        ResetGame();
        SceneTransitionManager.Instance.LoadScene("Main Menu", "CrossFade");
    }

    public void ResetGame()
    {
        _level = 1;  
    }
}
