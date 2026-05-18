using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

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
        GlobalEvents.OnChangeScene.AddListener(NextLevel);
    }

    private void OnDisable()
    {
        GlobalEvents.OnChangeScene.RemoveListener(NextLevel);
    }

    public void NextLevel(int next)
    {
        Debug.Log($"Change Scene to {next}");
        SceneManager.LoadScene(next);
    }
}
