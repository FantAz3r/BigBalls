using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class SkeletonSpawnAnimation : MonoBehaviour, ISpawnAnimation
{
    [SerializeField] private SkeletonKing _skeletonKing;

    [SerializeField] private float _moveSpeed = 1f;
    [SerializeField] private float _spawnOffsetZ = 12f;
    [SerializeField] private Ease _ease = Ease.Linear;

    private Tween _moveTween;
    private Vector3 _startPosition;
    private Vector3 _targetPosition;

    private void OnEnable ()
    {
        _skeletonKing.Spawned += Stop;
    }

    public void PlaySpawn (Vector3 spawnPoint, bool wantResetTransform = true)
    {
        _targetPosition = spawnPoint;
        _startPosition = spawnPoint + new Vector3(0f, 0f, _spawnOffsetZ);
        transform.position = _startPosition;

        float distance = Vector3.Distance(_startPosition, _targetPosition);
        float duration = distance / Mathf.Max(0.01f, _moveSpeed);

        _moveTween?.Kill();

        _moveTween = transform
            .DOMove(_targetPosition, duration)
            .SetEase(_ease)
            .SetUpdate(UpdateType.Normal);
    }

    public void Stop ()
    {
        if (_moveTween != null && _moveTween.IsActive())
            _moveTween.Kill(complete: false);

        _skeletonKing.Spawned -= Stop;
    }
}