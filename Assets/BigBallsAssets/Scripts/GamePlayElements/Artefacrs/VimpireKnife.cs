using System;
using BigBalls.Factories;
using BigBalls.GameplayObjects;
using BigBalls.Services;
using DG.Tweening;
using UnityEngine;
using VContainer;

public class VimpireKnife : EffectBehaviour
{
    private VimpireKnifeConfig _config;
    private IPlayerProvider _playerProvider;
    private IEntityRepository _entityRepository;
    private IEnemyFactory _enemyFactory;
    private IPoolService _poolService;
    private IDamageService _damageService;
    private Tween _jumpTween;

    public VimpireKnife (VimpireKnifeConfig config, int level) : base(config, level)
    {
        _config = config;
    }

    [Inject]
    public void Construct (
        IPlayerProvider playerProvider,
        IEntityRepository entityRepository,
        IEnemyFactory enemyFactory,
        IPoolService poolService,
        IDamageService damageService)
    {
        _entityRepository = entityRepository;
        _playerProvider = playerProvider;
        _enemyFactory = enemyFactory;
        _poolService = poolService;
        _damageService = damageService;
    }

    private void OnShoot ()
    {
        _damageService.ApplySelfDamage(_playerProvider.Player, _config.DamagePerShot);
    }

    private void OnKill (IEntity entity)
    {
        View(_playerProvider.Player, entity);
    }

    protected override IDisposable SubscribeInternal (IEntity host)
    {
        Shooter shooter = _entityRepository.GetContainer(_playerProvider.Player).Get<Shooter>();
        shooter.Shooted += OnShoot;
        _enemyFactory.Killed += OnKill;

        return new DisposableObject(() =>
        {
            _jumpTween?.Kill();
            shooter.Shooted -= OnShoot;
            _enemyFactory.Killed -= OnKill;
        });
    }

    public void View (Player player, IEntity enemy)
    {
        ParticleObject particleObject = _poolService.Get(_config.Particle);
        particleObject.Play();
        Transform target = player.transform;
        Vector3 start = enemy.Transform.position;

        particleObject.transform.position = start;

        float duration = 0.6f;
        float jumpPower = 3.5f;

        _jumpTween = particleObject.transform
            .DOJump(target.position, jumpPower, 1, duration)
            .SetEase(Ease.Linear)
            .OnUpdate(() =>
            {
            })
            .OnComplete(() =>
            {
                particleObject.Stop();
                _damageService.ApplyHeal(_playerProvider.Player, _config.GetHeal(Level), Color.green);
                _poolService.Release(particleObject);
            });
    }
}
