using System;
using BigBalls.Factories;
using BigBalls.GameplayObjects;
using DG.Tweening;
using UnityEngine;
using VContainer;

public class DieAnimation : MonoBehaviour
{
    [SerializeField] private ParticleObject _deathParticle;

    private float _jumpForce = 1.75f;
    private float _rotationAngle = 60f;
    private float _animationDuration = 0.45f;

    private SoundSpawner _soundSpawner;
    private IParticleFactory _particleFactory;
    private float _offsetY;
    private Quaternion _rotation;

    [Inject]
    public void Construct (IParticleFactory particleFactory, SoundSpawner soundSpawner)
    {
        _particleFactory = particleFactory;
        _soundSpawner = soundSpawner;
    }

    private void OnEnable ()
    {
        _offsetY = transform.position.y;
        _rotation = transform.rotation;
    }

    public void Play (Enemy enemy, Action<Enemy> callback)
    {
        float randomSide = UnityEngine.Random.value > 0.5f ? 1f : -1f;
        float targetZRotation = _rotationAngle * randomSide;

        Sequence deathSequence = DOTween.Sequence();

        deathSequence.Append(transform.DOMoveY(_offsetY + _jumpForce, _animationDuration / 2)
            .SetEase(Ease.OutQuad));
        deathSequence.Join(transform.DORotate(new Vector3(0, 0, targetZRotation), _animationDuration / 2)
            .SetEase(Ease.OutQuad));
        deathSequence.Append(transform.DOMoveY(_offsetY, _animationDuration / 2)
            .SetEase(Ease.InQuad));
        deathSequence.Join(transform.DOScale(Vector3.zero, _animationDuration / 2)
            .SetEase(Ease.InBack));

        deathSequence.AppendCallback(() =>
        {
            if (_deathParticle != null)
            {
                _particleFactory.Create(_deathParticle, transform.position);
            }
        });

        deathSequence.OnComplete(() =>
        {
            callback?.Invoke(enemy);
            _soundSpawner.Spawn(enemy.Config.DieSound);
            transform.rotation = _rotation;
        });
    }
}
