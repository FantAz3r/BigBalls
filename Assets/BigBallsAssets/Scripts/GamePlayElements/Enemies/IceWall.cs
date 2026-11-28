using System.Collections.Generic;
using BigBalls.Factories;
using BigBalls.GameplayObjects;
using BigBalls.Infrastructure.DI;
using BigBalls.Services;
using DG.Tweening;
using UnityEngine;
using VContainer;

public class IceWall : MonoBehaviour
{
    private const float OffsetY = 0.5f;

    [SerializeField] private Ball _prefab;
    [SerializeField] private float _spreadAngle;
    [SerializeField] private int _projectileCount = 5;

    [Header("Fall Animation (DOTween)")]
    [SerializeField] private float _fallHeight = 6f;
    [SerializeField] private float _fallDuration = 0.35f;
    [SerializeField] private Ease _fallEase = Ease.InQuad;
    [SerializeField] private float _landDuration = 0.18f;
    [SerializeField] private Ease _landEase = Ease.OutQuad;
    [SerializeField] private float _landSquash = 0.25f;

    private IPoolService _poolService;
    private IBallFactory _ballFactory;
    private IIdentifierService _identifierService;
    private IObjectResolverProvider _objectResolverProvider;

    private Vector3 _originalPosition;
    private Vector3 _originalScale;
    private Sequence _fallSequence;

    [Inject]
    public void Construct (
        IPoolService poolService,
        IBallFactory ballFactory,
        IIdentifierService identifierService,
        IObjectResolverProvider objectResolverProvider)
    {
        _poolService = poolService;
        _ballFactory = ballFactory;
        _identifierService = identifierService;
        _objectResolverProvider = objectResolverProvider;
    }

    private void Awake ()
    {
        _originalScale = transform.localScale;
    }

    public void Play()
    {
        _originalPosition = transform.position;
        _originalScale = transform.localScale;

        PlayFallAnimation();
    }

    private void OnDisable ()
    {
        _fallSequence?.Kill();
        _fallSequence = null;

        transform.position = _originalPosition;
        transform.localScale = _originalScale;
    }

    private void PlayFallAnimation ()
    {
        _fallSequence?.Kill();

        Vector3 fallStart = _originalPosition + Vector3.up * _fallHeight;
        transform.position = fallStart;
        transform.localScale = _originalScale;

        float squashY = 1f - _landSquash;
        float stretchXZ = 1f + _landSquash * 0.5f;

        Vector3 squashedScale = new Vector3(
            _originalScale.x * stretchXZ,
            _originalScale.y * squashY,
            _originalScale.z * stretchXZ
        );

        _fallSequence = DOTween.Sequence()
            .Append(transform.DOMove(_originalPosition, _fallDuration).SetEase(_fallEase))
            .Append(transform.DOScale(squashedScale, _landDuration * 0.5f).SetEase(_landEase))
            .Append(transform.DOScale(_originalScale, _landDuration * 0.5f).SetEase(Ease.OutBack))
            .SetLink(gameObject)
            .OnComplete(() => _fallSequence = null);
    }

    public void BreakWall ()
    {
        for (int i = 0; i < _projectileCount; i++)
        {
            BallModel ballModel = new BallModel(_identifierService.ID, _prefab.Config);
            Ball ball = _ballFactory.Create(ballModel, EntityType.Enemy, transform, CreateStrategies());

            float angleStep = _projectileCount > 1 ? _spreadAngle / (_projectileCount - 1) : 0;
            float currentAngle = -(_spreadAngle / 2f) + (i * angleStep);

            Quaternion rotation = Quaternion.Euler(0, currentAngle, 0);
            Vector3 direction = transform.rotation * rotation * Vector3.forward;

            ball.transform.position = new Vector3(transform.position.x, OffsetY, transform.position.z);
            ball.transform.rotation = rotation * transform.rotation;
            ball.Subscribe();

            ball.Mover.SetDirection(direction);
        }
    }

    private void OnBecameInvisible ()
    {
        _poolService.Release(this);
    }

    private List<ICollisionStrategy> CreateStrategies ()
    {
        List<ICollisionStrategy> strategyList = new List<ICollisionStrategy>()
        {
             new HitPlayerStrategy(),
             new ReflectCollisionStrategy(),
             new BallCollisionStrategy()
        };

        foreach (ICollisionStrategy strategy in strategyList)
            _objectResolverProvider.CurrentResolver.Inject(strategy);

        return strategyList;
    }
}