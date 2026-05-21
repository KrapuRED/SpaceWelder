using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class TypeWriterEffect : TypeEffect
{  
    private bool _readyForNewText = true;

    [Header("Type Writer Effect Configuration")]
    [SerializeField] private CharacterType charType;
    [SerializeField] private TextMeshProUGUI textBox;
    [SerializeField] private float charactertPerSeconds = 20;
    [SerializeField] private float interpuncuationDelay = 0.5f;
    private int _currentVisibleCharacterIndex;
    private Coroutine _typeWriterCoroutine;

    private WaitForSeconds _simpleyDelay;
    private WaitForSeconds _interpuncuationDelay;

    [Header("Skip Option")]
    [SerializeField] private bool quickSkipEnabled;
    [SerializeField][Min(1)] private int skipSpeedUp = 5;
    public bool CurrentlySkipping { get; private set; }
    private WaitForSeconds _skipDelay;

    [Header("Event Functional")]
    private WaitForSeconds _textBoxFullEventDelay;
    [SerializeField]
    [Range(0.1f, 0.5f)] private float textBoxFullEventDelayTime = 0.2f;


    private void Awake()
    {
        _simpleyDelay = new WaitForSeconds(1f / charactertPerSeconds);
        _interpuncuationDelay = new WaitForSeconds(interpuncuationDelay);

        _skipDelay = new WaitForSeconds(1f / (charactertPerSeconds * skipSpeedUp));
        _textBoxFullEventDelay = new WaitForSeconds(textBoxFullEventDelayTime);
    }

    public override void PlayText(string newText)
    {
        if (!_readyForNewText) return;

        Debug.Log(newText);

        textBox.text = newText;
        StartTyping();
    }

    private  void StartTyping()
    {
        IsTyping = true;
        _readyForNewText = false;

        if (_typeWriterCoroutine != null)
            StopCoroutine(_typeWriterCoroutine);

        textBox.maxVisibleCharacters = 0;
        _currentVisibleCharacterIndex = 0;

        textBox.ForceMeshUpdate();

        _typeWriterCoroutine = StartCoroutine(TypeWriting());
    }

    IEnumerator TypeWriting()
    {
        TMP_TextInfo textInfo = textBox.textInfo;

        while (_currentVisibleCharacterIndex < textInfo.characterCount)
        {
            //Event Area
            if (CurrentlySkipping)
                SoundEffectManager.Instance.StopSoundEffect();
            else
            {
                if (charType == CharacterType.Captian)
                {
                    SoundEffectManager.Instance.PlaySoundEffect("CapBlipEffect");

                }
                else
                {
                    SoundEffectManager.Instance.PlaySoundEffect("0therBlipEffect");
                }
            }

            var lastCharacterIndex = textInfo.characterCount - 1;

            if (_currentVisibleCharacterIndex == lastCharacterIndex)
            {
                textBox.maxVisibleCharacters++;
                yield return _textBoxFullEventDelay;
                GlobalEvents.OnCompleteTextRevealed.Invoke();

                _readyForNewText = true;
                IsTyping = false;
                yield break;
            }

            //Call Sound Typin Effect Here

            char character = textInfo.characterInfo[_currentVisibleCharacterIndex].character;

            textBox.maxVisibleCharacters++;

            if (!CurrentlySkipping &&
                (character == '?' || character == '.' || character == ',' || character == ':' || character == ';' ||
                character == '!' || character == '-'))
            {
                yield return _interpuncuationDelay;
            }
            else
            {
                yield return CurrentlySkipping ? _skipDelay : _simpleyDelay;
            }

            _currentVisibleCharacterIndex++;
        }
    }
    public override void Skip()
    {
        if (textBox.maxVisibleCharacters >= textBox.textInfo.characterCount - 1)
        {
            return;
        }

        if (CurrentlySkipping)
            return;

        CurrentlySkipping = true;

        if (!quickSkipEnabled)
        {
            StartCoroutine(SkipSpeedUpReset());
            return;
        }

        StopCoroutine(_typeWriterCoroutine);
        textBox.maxVisibleCharacters = textBox.textInfo.characterCount;
        IsTyping = false;
        _readyForNewText = true;
        GlobalEvents.OnCompleteTextRevealed.Invoke();
    }

    IEnumerator SkipSpeedUpReset()
    {
        yield return new WaitUntil(() => textBox.maxVisibleCharacters == textBox.textInfo.characterCount - 1);
        CurrentlySkipping = false;
    }

}
