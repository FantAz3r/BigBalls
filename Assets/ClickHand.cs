using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ClickHand : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ParticleObject _particle;
    [SerializeField] private Image _hand;

    [Header("Rotation Settings")]
    [SerializeField] private float _rotateAmount = -40f;
    [SerializeField] private float _rotateForwardTime = 0.2f;
    [SerializeField] private Ease _easeForward;
    [SerializeField] private Ease _easeBackward;
    [SerializeField] private float _rotateBackTime = 0.4f;
    [SerializeField] private float _rotateLoopDelay = 0.3f;
    [SerializeField] private float _moveDuration = 1;


    [Header("Move Settings")]
    [SerializeField] private float _moveLoopDelay = 0.3f;

    private Sequence _currentSequence;

    private void Awake ()
    {
        _hand.transform.rotation = Quaternion.Euler(0, 0, _rotateAmount);
        _hand.transform.localScale = Vector3.one;
    }

    public void PlayClick (Vector3 position)
    {

        Sequence moveSeq = CreateChainNoReset();
        AppendMoveTo(moveSeq, position);
        AppendDelay(moveSeq, _moveLoopDelay);

        moveSeq.OnComplete(() =>
        {
            Sequence clickSeq = CreateChainNoReset();
            AppendDelay(clickSeq, _rotateLoopDelay);

            AppendForward(clickSeq);
            AppendBackward(clickSeq);
            clickSeq.SetLoops(-1, LoopType.Restart);
            clickSeq.Play();
        });

        moveSeq.Play();
    }

    public void PlayDragItem (UIItem iItem, Vector3 position, Slot target)
    {
        UIItem clone = null;

        Vector3 handPosition = transform.localPosition;

        Sequence dragSeq = CreateChainNoReset();
        AppendForward(dragSeq);
        AppendAction(dragSeq, () =>
        {
            clone = null;
            clone = Instantiate(iItem, _hand.transform);

            RectTransform rect = clone.GetComponent<RectTransform>();

            if (rect != null)
            {
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;
                rect.pivot = new Vector2(0.5f, 0.5f);

                rect.localPosition = Vector3.zero;
                rect.localRotation = Quaternion.identity;
                rect.localScale = Vector3.one * 0.75f;
            }

            clone.CanvasGroup.alpha = 0.7f;
            clone.HighliterUI.Hide();
        });

        AppendMoveTo(dragSeq, position);

        AppendAction(dragSeq, () =>
        {
            clone.transform.SetParent(target.transform);
            clone.transform.localPosition = Vector3.zero;
            clone.transform.rotation = Quaternion.identity;
            clone.transform.localScale = Vector3.one * 0.75f;
            Destroy(clone.gameObject, 1f);
        });

        AppendBackward(dragSeq);
        AppendMoveTo(dragSeq, handPosition);

        dragSeq.SetLoops(-1, LoopType.Restart);
        dragSeq.Play();
        dragSeq.OnKill(() => Destroy(clone.gameObject));
    }

    private void OnDisable ()
    {
        KillAndReset();
    }

    // ============================================================
    //  BUILDER API Ч собираем цепочку вручную
    // ============================================================

    public Sequence CreateChain (bool autoPlay = true)
    {
        _currentSequence = DOTween.Sequence();

        if (autoPlay)
            _currentSequence.Play();

        return _currentSequence;
    }

    public void PlayChain (Sequence sequence)
    {
        KillAndReset();
        _currentSequence = sequence;
        _currentSequence.Play();
    }

    // ============================================================
    //  CHAINABLE STEPS Ч добавл€ют шаги в переданную Sequence
    //  и возвращают еЄ же дл€ fluent-чейнинга
    // ============================================================

    public Sequence AppendBackward (Sequence seq)
    {


        Tween backward = _hand.transform
           .DORotate(new Vector3(0, 0, _rotateAmount), _rotateBackTime)
           .SetEase(_easeBackward);

        seq.Append(backward);

        return seq;
    }

    public Sequence AppendForward (Sequence seq)
    {

        Tween forward = _hand.transform
            .DORotate(Vector3.zero, _rotateForwardTime)
            .SetEase(_easeForward)
            .OnComplete(() =>
            {
                _particle.Play();
            });

        seq.Append(forward);

        return seq;
    }

    public Sequence AppendMoveTo (Sequence seq, Vector3 targetPosition, bool isLocal = true)
    {
        Tween forward = isLocal
            ? transform.DOLocalMove(targetPosition, _moveDuration).SetEase(Ease.InOutSine)
            : transform.DOMove(targetPosition, _moveDuration).SetEase(Ease.InOutSine);

        seq.Append(forward);
        return seq;
    }

    public Sequence AppendDelay (Sequence seq, float seconds)
    {
        seq.AppendInterval(seconds);
        return seq;
    }

    public Sequence AppendAction (Sequence seq, Action action)
    {
        seq.AppendCallback(() => action());
        return seq;
    }

    private void KillAndReset ()
    {
        _currentSequence?.Kill();
        _currentSequence = null;
    }

    public Sequence CreateChainNoReset (bool autoPlay = false)
    {
        _currentSequence?.Kill();
        _currentSequence = DOTween.Sequence();

        if (autoPlay)
            _currentSequence.Play();
        return _currentSequence;
    }

    public void StopAllAnimations () => KillAndReset();
}