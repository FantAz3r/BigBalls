using System;
using DG.Tweening;
using UnityEngine;

public class EnemyFallAnimator : MonoBehaviour
{
    [Header("Fall Settings")]
    [SerializeField] private float _startOffsetY = 10f;
    [SerializeField] private float _fallDuration = 0.8f;
    [SerializeField] private float _delay = 0f;
    [SerializeField] private Ease _fallEase = Ease.OutBounce;
    [SerializeField] private float _landingBounce = 0.15f;
    [SerializeField] private float _landingBounceDuration = 0.2f;

    private Vector3 _targetPosition;
    private Tween _fallTween;

    public event Action OnFallComplete;

    public void PlayFall (Vector3 spawnPoint, bool wantResetTransform = true)
    {
        _fallTween?.Kill();
        _targetPosition = spawnPoint;
        if (wantResetTransform)
        {
            Vector3 startPos = _targetPosition + Vector3.up * _startOffsetY;
            transform.position = startPos;
        }

        Sequence seq = DOTween.Sequence();
        seq.SetUpdate(true);
        seq.Join(transform.DOMove(_targetPosition, _fallDuration).SetEase(_fallEase));

        if (_landingBounce > 0f)
        {
            Vector3 bounceUp = _targetPosition + Vector3.up * _landingBounce;
            seq.Append(transform.DOMove(bounceUp, _landingBounceDuration).SetEase(Ease.OutQuad));
            seq.Append(transform.DOMove(_targetPosition, _landingBounceDuration).SetEase(Ease.InQuad));
        }

        if (_delay > 0f)
        {
            seq.PrependInterval(_delay);
        }

        seq.OnComplete(() =>
        {
            OnFallComplete?.Invoke();
        });

        _fallTween = seq;
        seq.Play();
    }

    public void CancelAndSnapToTarget ()
    {
        _fallTween?.Kill();
        transform.position = _targetPosition;
    }

    private void OnDisable ()
    {
        _fallTween?.Kill();
    }
}
