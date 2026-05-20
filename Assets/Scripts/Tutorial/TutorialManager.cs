using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private List<Tutorial> _tutorials = new();

    private int _tutorialCount = 0;
    private HashSet<Tutorial> _completeTutorials = new();

    private void Start()
    {
        if (!GameManager.Instance.IsTutorialComplete) return;

        if (_tutorials.Count > 0)
        {
            GlobalEvents.OnStartHullBreachGame.Invoke();
            PauseGame();
            _tutorials[_tutorialCount].ShowTutorial();
        }
    }

    public void OnContinueTutorial(InputAction.CallbackContext contex)
    {
        if (contex.started && _tutorialCount < _tutorials.Count)
            OnCompleteTutorial();
    }

    private void OnCompleteTutorial()
    {
        _tutorials[_tutorialCount].HideTutorial();

        _tutorialCount++;

        if (_tutorialCount >= _tutorials.Count)
        {
            ResumeGame();

            GameManager.Instance.OnTutorialComplete();
            GlobalEvents.OnTutorialComplete.Invoke();
            return;
        }

        _tutorials[_tutorialCount].ShowTutorial();
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;
    }
}
