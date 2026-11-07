using System;
using System.Collections.Generic;
using BigBalls.Factories;
using BigBalls.GameplayObjects;
using BigBalls.Infrastructure.DI;
using BigBalls.Services;
using DG.Tweening;
using UnityEngine;
using VContainer;

[RequireComponent(typeof(Collider))]
public class Hand : MonoBehaviour
{
    [SerializeField] private Ball _handProjectile;
    [SerializeField] private float _shootDelay = 0.15f;

    private Vector3 _homeLocalPosition;
    private Quaternion _homeLocalRotation;
    private Sequence _tween;
    private SkeletonKingConfig _config;
    private IBallFactory _ballFactory;
    private IIdentifierService _identifierService;
    private IObjectResolverProvider _objectResolverProvider;
    private IDamageService _damageService;
    private Collider _collider;
    private Transform _parent;
    public bool IsBusy { get; private set; }

    private void Awake ()
    {
        _collider = GetComponent<Collider>();
        _parent = transform.parent;
    }

    private void OnTriggerEnter (Collider other)
    {
        if (IsBusy)
        {
            if (other.gameObject.TryGetComponent(out Player player))
            {
                _damageService.ApplyDamage(player, _config.HandDamage, Color.red);
            }
        }
    }

    private void OnDestroy ()
    {
        _tween?.Kill();
    }

    [Inject]
    public void Construct (IBallFactory ballFactory, IIdentifierService identifierService, IDamageService damageService, IObjectResolverProvider objectResolverProvider)
    {
        _ballFactory = ballFactory;
        _identifierService = identifierService;
        _damageService = damageService;
        _objectResolverProvider = objectResolverProvider;
    }

    public void Punch (SkeletonKingConfig config, Vector3 worldDirection, float distance, float travelTime, float returnTime, Action onFinished = null)
    {
        if (IsBusy)
            return;

        _config = config;
        IsBusy = true;
        _collider.isTrigger = true;
        _homeLocalPosition = transform.localPosition;
        _homeLocalRotation = transform.localRotation;
        transform.SetParent(null);
        _tween?.Kill();


        Vector3 startWorld = transform.position;
        Vector3 dir = worldDirection;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.0001f)
            dir = transform.forward;

        dir.Normalize();
        Quaternion lookRot = Quaternion.LookRotation(dir, Vector3.up) * Quaternion.Euler(0f, -90f, 0f);

        Vector3 targetWorld = startWorld + dir * distance;
        Vector3 targetLocal = transform.parent != null
            ? transform.parent.InverseTransformPoint(targetWorld)
            : targetWorld;

        Vector3 left = new Vector3(-dir.z, 0f, dir.x).normalized;

        _tween = DOTween.Sequence();
        _tween.Append(transform.DOLocalMove(targetLocal, travelTime).SetEase(Ease.OutQuad));
        _tween.SetUpdate(UpdateType.Late);

        for (float t = 0f; t < travelTime; t += _shootDelay)
        {
            _tween.InsertCallback(t, () =>
            {
                FireBall(left);
                FireBall(-left);
            });
        }

        _tween.Append(transform.DOLocalMove(_homeLocalPosition, returnTime).SetEase(Ease.InQuad));
        _tween.OnComplete(() =>
        {
            transform.SetParent(_parent);
            transform.localPosition = _homeLocalPosition;
            transform.localRotation = _homeLocalRotation;
            IsBusy = false;
            onFinished?.Invoke();
            _collider.isTrigger = false;
        });
        _tween.OnKill(() =>
        {
            transform.SetParent(_parent);
            transform.localPosition = _homeLocalPosition;
            transform.localRotation = _homeLocalRotation;
            IsBusy = false;
            onFinished?.Invoke();
            _collider.isTrigger = false;
        })

        .SetLink(gameObject);
    }

    public void ResetToHome ()
    {
        _tween?.Kill();
        _tween = null;
        IsBusy = false;
        transform.localPosition = _homeLocalPosition;
        transform.localRotation = _homeLocalRotation;
    }

    private void FireBall (Vector3 direction)
    {
        BallModel ballModel = new BallModel(_identifierService.ID, _handProjectile.Config);
        var ball = _ballFactory.Create(ballModel, EntityType.Enemy, transform, CreateCollisionStrategies(), Quaternion.LookRotation(direction));
        ball.Mover.SetDirection(direction);
    }

    private List<ICollisionStrategy> CreateCollisionStrategies ()
    {
        List<ICollisionStrategy> collisionStrategies = new List<ICollisionStrategy>
        {
            new BallCollisionStrategy(),
            new HitPlayerStrategy(),
            new WallColllisionStrategy()
        };

        foreach (ICollisionStrategy strategy in collisionStrategies)
            _objectResolverProvider.CurrentResolver.Inject(strategy);

        return collisionStrategies;
    }
}