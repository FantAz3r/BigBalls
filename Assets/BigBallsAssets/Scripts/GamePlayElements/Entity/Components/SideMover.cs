using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SideMover : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _distance = 1f;
    [SerializeField] private float _moveDuration = 0.3f;
    [SerializeField] private float _interval = 3f;
    [SerializeField] private Ease _ease = Ease.Linear;

    [Header("Check")]
    [SerializeField] private float _checkDistance = 1f;
    [SerializeField] private LayerMask _blockingMask;

    private Tween _tween;
    private Coroutine _loopRoutine;
    private WaitForSeconds _wait;

    private void OnEnable ()
    {
        _wait = new WaitForSeconds(_interval);
        _loopRoutine = StartCoroutine(MoveLoop());
    }

    private void OnDisable ()
    {
        if (_loopRoutine != null)
        {
            StopCoroutine(_loopRoutine);
            _loopRoutine = null;
        }

        _tween?.Kill();
        _tween = null;
    }

    private IEnumerator MoveLoop ()
    {
        while (true)
        {
            yield return _wait;

            Vector3 direction = PickDirection();

            if (direction == Vector3.zero)
                continue;

            bool done = false;

            _tween = transform
                .DOMoveX(transform.position.x + direction.x * _distance, _moveDuration)
                .SetEase(_ease)
                .SetUpdate(UpdateType.Normal)
                .OnComplete(() => done = true);

            yield return new WaitUntil(() => done);
        }
    }

    private Vector3 PickDirection ()
    {
        List<Vector3> candidates = new List<Vector3>(2);

        if (IsBlocked(Vector3.right) == false)
            candidates.Add(Vector3.right);

        if (IsBlocked(Vector3.left) == false)
            candidates.Add(Vector3.left);

        if (candidates.Count == 0)
            return Vector3.zero;

        return candidates[Random.Range(0, candidates.Count)];
    }

    private bool IsBlocked (Vector3 direction)
    {
        return Physics.Raycast(
            transform.position,
            direction,
            _checkDistance,
            _blockingMask,
            QueryTriggerInteraction.Ignore);
    }
}
