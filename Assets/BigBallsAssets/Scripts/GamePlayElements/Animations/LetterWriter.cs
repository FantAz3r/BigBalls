using System.Collections;
using System.Collections.Generic;
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

    public void StartTyping ()
    {
        _typingTween?.Kill();

        _textComponent.maxVisibleCharacters = 0;

        DOVirtual.DelayedCall(0, () =>
        {
            int totalCharacters = _textComponent.textInfo.characterCount;

            float duration = totalCharacters * _delayBetweenLetters;

            _typingTween = DOTween.To(
                    () => _textComponent.maxVisibleCharacters, // Геттер
                    x => _textComponent.maxVisibleCharacters = x, // Сеттер
                    totalCharacters, // Конечное значение
                    duration) // Длительность
                .SetEase(Ease.Linear) // Линейная скорость, чтобы буквы шли равномерно
                .SetUpdate(true); // true = не зависит от Time.timeScale (работает во время паузы)
        });
    }

    private void OnDisable ()
    {
        _typingTween?.Kill();
    }
}
