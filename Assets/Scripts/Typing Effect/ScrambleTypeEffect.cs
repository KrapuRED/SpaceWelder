using TMPro;
using UnityEngine;
using System.Collections;

public class ScrambleTypeEffect : TypeEffect
{
    [SerializeField] private TextMeshProUGUI textBox;
    [SerializeField] private float revealPerSecond = 1f;
    [SerializeField] private float scrambleSpeed = 0.05f;
    [SerializeField] private int scrambleFramesPerChar = 5;

    private const string RandomChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%";
    private Coroutine _scrambleCoroutine;
    private bool _readyForNewText = true;
    private string _targetText;
       
    private int _currentVisibleCharacterIndex;

    private WaitForSeconds _scrambleDelay;
    private WaitForSeconds _revealDelay;

    private void Awake()
    {
        _scrambleDelay = new WaitForSeconds(scrambleSpeed);
        _revealDelay = new WaitForSeconds(1f / revealPerSecond);
    }

    public override void PlayText(string newText)
    {
        if (!_readyForNewText) return;

        _targetText = newText;
        StartScramble();
    }

    private void StartScramble()
    {
        IsTyping = true;
        _readyForNewText = false;

        if (_scrambleCoroutine != null)
            StopCoroutine(_scrambleCoroutine);

        _scrambleCoroutine = StartCoroutine(ScrambleReveal());
    }

    IEnumerator ScrambleReveal()
    {
        int totalLength = _targetText.Length;
        int revealedCount = 0;
        char[] buffer = new char[totalLength];

        while (revealedCount < totalLength)
        {
            // Scramble N times before locking next char
            for (int frame = 0; frame < scrambleFramesPerChar; frame++)
            {
                for (int i = 0; i < revealedCount; i++)
                    buffer[i] = _targetText[i];                          // locked

                for (int i = revealedCount; i < totalLength; i++)
                    buffer[i] = RandomChars[Random.Range(0, RandomChars.Length)]; // scrambled

                textBox.text = new string(buffer);
                yield return _scrambleDelay; // single cached delay
            }

            revealedCount++;
        }

        textBox.text = _targetText;
        IsTyping = false;
        _readyForNewText = true;
        GlobalEvents.OnCompleteTextRevealed.Invoke();
    }

    public override void Skip()
    {
        if (!IsTyping) return;

        StopCoroutine(_scrambleCoroutine);
        textBox.text = _targetText;
        IsTyping = false;
        _readyForNewText = true;
        GlobalEvents.OnCompleteTextRevealed.Invoke();
    }

}
