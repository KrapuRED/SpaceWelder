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

    private void OnEnable()
    {
        GlobalEvents.OnPlayerDeath.AddListener(BackToMainMenu);
    }

    private void OnDisable()
    {
        GlobalEvents.OnPlayerDeath.RemoveListener(BackToMainMenu);
    }

    private void OnDestroy()
    {
        GlobalEvents.OnPlayerDeath.RemoveListener(BackToMainMenu);
    }

    public void OnTutorialComplete()
    {
        _isTutorialComplete = true;
    }

    public void StartGame()
    {
        SceneTransitionManager.Instance.LoadScene($"Main-GamePlay-Story{_level}", "CrossFade");
        //SceneManager.LoadScene($"Main-GamePlay-Story{_level}");
    }

    public void NextStory()
    {
        _level++;
        SceneTransitionManager.Instance.LoadScene($"Main-GamePlay-Story{_level}", "CrossFade");
    }

    public void NextLevel()
    {
        if (_level >= 4)
        {
            Debug.Log("Congratulations! You've completed all levels!");
            return;
        }

        MusicManager.Instance.PlayMusic(gamePlayLevelBGM);
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
