using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    public GameObject loadingBar;
    public Slider loadingProgress;

    public GameObject transitionsContainer;

    private SceneTransition[] transitions;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        transitions = transitionsContainer.GetComponentsInChildren<SceneTransition>(true);
    }

    public void LoadScene(string sceneName, string transitionName)
    {
        StartCoroutine(LoadSceneAsync(sceneName, transitionName));
    }

    private IEnumerator LoadSceneAsync(string sceneName, string transitionName)
    {
        SceneTransition transition = transitions.First(t => t.name == transitionName);

        if (transition == null)
        {
            Debug.LogError($"Transition with name {transitionName} not found.");
            yield break;
        }

        AsyncOperation scene = SceneManager.LoadSceneAsync(sceneName);

        scene.allowSceneActivation = false;

        yield return transition.AnimateTransitionIn();

        loadingBar.SetActive(true);

        do
        {
            loadingProgress.value = scene.progress;
            yield return null;
        } while (scene.progress < 0.9f);

        yield return new WaitForSeconds(1f);

        scene.allowSceneActivation = true;

        loadingBar.SetActive(false);

        yield return transition.AnimateTransitionOut();
    }
}
