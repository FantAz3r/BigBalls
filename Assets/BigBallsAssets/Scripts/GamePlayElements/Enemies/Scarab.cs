using System.Collections.Generic;
using BigBalls.Factories;
using BigBalls.GameplayObjects;
using BigBalls.Infrastructure.DI;
using BigBalls.Services;
using DG.Tweening;
using UnityEngine;
using VContainer;

public class Scarabey : Enemy
{
    [SerializeField] private int _maxHitCount;
    [SerializeField] private float _ballStateSpeed;
    [SerializeField] private float _ballStateDamage;
    [SerializeField] private Ball _ball;
    [SerializeField] private float _rollSpeed;
    [SerializeField] private List<float> _healthThresholds;
    [SerializeField] private float _rotationAngle = 360f;
    [SerializeField] private float _rotationDuration = 0.3f;
    [SerializeField] private Collider _collider;
    [SerializeField] private Collider _ballCollider;
    [SerializeField] private Transform _sphereModel;
    [SerializeField] private Transform _defaultModel;

    private int _currentHitCount;
    private Stat _health;
    private int _tresholdIndex = 0;
    private IPlayerProvider _playerProvider;
    private IIdentifierService _identifierService;
    private IEffectFactory _effectFactory;
    private IEntityRepository _entityRepository;
    private Tween _rotationTween;
    private IObjectResolver _objectResolver;
    private Vector2 _enemyDirection;
    private Quaternion _initialRotation;

    private void OnDestroy ()
    {
        if (_health != null)
            _health.ValueChanged -= OnHealthChange;
    }

    [Inject]
    public void Construct (
        IDamageService damageService,
        IPlayerProvider playerProvider,
        IIdentifierService identifierService,
        IUpdateService updateService,
        IEffectFactory effectFactory,
        IEntityRepository entityRepository,
        IObjectResolverProvider objectResolverProvider)
    {
        _identifierService = identifierService;
        _playerProvider = playerProvider;
        _effectFactory = effectFactory;
        _entityRepository = entityRepository;
        _objectResolver = objectResolverProvider.CurrentResolver;
    }

    protected override void ChildInit ()
    {
        _initialRotation = transform.rotation;
        InitBall(_ball);
        _ball.Subscribe();
        _ball.Mover.SetMovableObject(transform);
        _health = StatHolder[StatType.Health];
        _health.ValueChanged += OnHealthChange;
        EnterDefaultState();
    }

    private Ball InitBall (Ball ball)
    {
        BallModel ballModel = new BallModel(_identifierService.ID, _ball.Config);
        StatHolder statHolder = new StatHolder(ball, _ball.Config.EntityType, _ball.Config);
        statHolder.InitStats();
        Mover mover = new Mover(statHolder[StatType.MoveSpeed], ball.transform);
        _objectResolver.Inject(mover);

        List<ISubscribable> subscribables = new List<ISubscribable>()
        {
            mover
        };

        DeathHandler<Ball> ballDeathHandler = new DeathHandler<Ball>(statHolder[StatType.Health], subscribables, ball);
        ballDeathHandler.Subscribe();

        var collisions = new List<ICollisionStrategy>()
        {
           new HitPlayerStrategy(),
           new ScarabCollisionStrategy()
        };

        ball.Construct(_identifierService.ID, ballModel, mover, collisions, ballDeathHandler, _effectFactory);
        _entityRepository.Add(ball, statHolder, null);
        return ball;
    }

    private void OnHealthChange (IReadonlyStat stat)
    {
        if (_tresholdIndex >= _healthThresholds.Count)
            return;

        if (stat.CurrentValue <= stat.MaxValue * _healthThresholds[_tresholdIndex])
        {
            _health.ValueChanged -= OnHealthChange;
            _tresholdIndex++;
            EnterBallState();
        }
    }

    private void EnterBallState ()
    {
        _enemyDirection = Mover.Direction;
        Mover.SetDirection(Vector2.zero);

        _ballCollider.enabled = true;
        _collider.enabled = false;
        _health.SetImmune(true);
        _currentHitCount = 0;
        _ball.EventHandler.Reflected += OnBallReflected;
        _ball.enabled = true;
        _defaultModel.gameObject.SetActive(false);
        OnBallReflected(this);
    }

    private void EnterDefaultState ()
    {
        if (_enemyDirection != Vector2.zero)
        {
            Mover.SetDirection(_enemyDirection);
        }


        _ball.Mover.SetDirection(Vector2.zero);
        _defaultModel.gameObject.SetActive(true);
        _ballCollider.enabled = false;
        _collider.enabled = true;
        _health.SetImmune(false);
        _health.ValueChanged += OnHealthChange;
    }

    private void OnBallReflected (IEntity useless)
    {
        transform.DOKill();
        transform.rotation = _initialRotation;

        if (_currentHitCount >= _maxHitCount)
        {
            _ball.EventHandler.Reflected -= OnBallReflected;
            _ball.enabled = false;
            EnterDefaultState();
            return;
        }

        _currentHitCount++;

        Vector3 playerPos = _playerProvider.Player.transform.position;
        Vector3 directionToPlayer = (playerPos - transform.position).normalized;

        Vector2 playerDirection2D = new Vector2(directionToPlayer.x, directionToPlayer.z);
        _ball.Mover.SetDirection(playerDirection2D);

        transform.DOKill();

        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

        transform.DORotateQuaternion(targetRotation, 0.5f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                _rotationTween = transform.DORotate(new Vector3(360f, transform.eulerAngles.y, transform.eulerAngles.z), 1f, RotateMode.FastBeyond360)
                    .SetEase(Ease.Linear)
                    .SetLoops(-1, LoopType.Incremental);
            });

        Debug.DrawRay(transform.position, directionToPlayer * 2f, Color.red);
    }
}
