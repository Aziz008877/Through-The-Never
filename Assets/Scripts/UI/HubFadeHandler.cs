using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class HubFadeHandler : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private UIFade _fade;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private PlayerState _playerState;
    [SerializeField] private GameObject _npcCanvas;
    [SerializeField] private TMP_Text _dialogueText;

    [Header("Config")]
    [SerializeField] private string[] _phrases;
    [SerializeField] private float _delayBeforeFade = 7.5f;
    [SerializeField] private float _delayAfterFade = 2f;
    [SerializeField] private float _phraseInterval = 4f;
    [SerializeField] private bool _hideCanvasWhenDone = true;

    private int _currentPhraseID = 0;
    private Coroutine _textRoutine;

    private IEnumerator Start()
    {
        _playerState.ChangePlayerState(false);

        // ждём до фейда
        yield return new WaitForSeconds(_delayBeforeFade);

        // фейд
        _fade.Fade(0);
        yield return new WaitForSeconds(_delayAfterFade);

        // запускаем диалог: включаем канвас, аудио и ТОЛЬКО ТЕПЕРЬ — смену текста
        _npcCanvas.SetActive(true);
        _audioSource.Play();

        // начать показ текста с первой фразы
        _currentPhraseID = 0;
        _textRoutine = StartCoroutine(ChangeText());

        // ждём, пока идёт озвучка (или пока кончатся фразы — что наступит раньше)
        float dialoguePlanned = Mathf.Max(_phraseInterval * Mathf.Max(1, _phrases?.Length ?? 0), 0f);
        float waitTime = _audioSource.clip ? Mathf.Max(_audioSource.clip.length, dialoguePlanned) : dialoguePlanned;
        yield return new WaitForSeconds(waitTime + 1f);

        // завершение
        if (_textRoutine != null) StopCoroutine(_textRoutine);
        _playerState.ChangePlayerState(true);
        if (_hideCanvasWhenDone) _npcCanvas.SetActive(false);
    }

    private IEnumerator ChangeText()
    {
        if (_phrases == null || _phrases.Length == 0 || _dialogueText == null)
            yield break;

        // показываем первую фразу сразу
        UpdateText();

        // далее — переключаем каждые _phraseInterval секунд
        while (true)
        {
            yield return new WaitForSeconds(_phraseInterval);

            // двигаем индекс и проверяем границы
            _currentPhraseID++;
            if (_currentPhraseID >= _phrases.Length)
                yield break; // фразы закончились

            UpdateText();
        }
    }

    private void UpdateText()
    {
        if (_phrases == null || _phrases.Length == 0 || _dialogueText == null) return;

        int clamped = Mathf.Clamp(_currentPhraseID, 0, _phrases.Length - 1);
        _dialogueText.text = _phrases[clamped];
    }
}
