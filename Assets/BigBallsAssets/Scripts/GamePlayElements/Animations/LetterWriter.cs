using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class LetterWriter : MonoBehaviour
{
    [SerializeField] private float _delayBetweenLetters = 0.05f;
    [SerializeField] private bool _playOnStart = true;

    private TMP_Text _textComponent;
    private Tween _typingTween;

    private void Awake ()
    {
        _textComponent = GetComponent<TMP_Text>();
    }

    private void OnEnable ()
    {
        if (_playOnStart)
        {
            StartTyping();
        }
    }

    public void StartTyping (Action action = null)
    {
        if (_textComponent == null)
            return;

        _typingTween?.Kill();
        _textComponent.ForceMeshUpdate();
        _textComponent.maxVisibleCharacters = 0;

        int totalCharacters = _textComponent.textInfo.characterCount;
        float duration = totalCharacters * _delayBetweenLetters;

        _typingTween = DOTween.To(
                () => _textComponent.maxVisibleCharacters,
                x => _textComponent.maxVisibleCharacters = x,
                totalCharacters,
                duration)
            .SetEase(Ease.Linear)
            .SetUpdate(true);
    }

    private void OnDisable ()
    {
        _typingTween?.Kill();
    }
}
