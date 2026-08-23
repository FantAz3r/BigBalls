using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BigBalls.Factories;
using BigBalls.GameplayObjects;
using BigBalls.Services;
using UnityEngine;
using VContainer;

public class Bomb : EffectBehaviour
{
    private readonly BombConfig _config;
    private IDamageService _damageService;
    private ISpatialService _spatialService;
    private ICoroutineRunner _coroutineRunner;
    private WaitForSeconds _respawnDelay;
    private IParticleFactory _particleFactory;

    public Bomb (BombConfig config, int level) : base(config, level)
    {
        _config = config;
        _respawnDelay = new WaitForSeconds(_config.RespawnDelay);
    }

    [Inject]
    public void Construct (IDamageService damageService, ISpatialService spatialService, ICoroutineRunner coroutineRunner, IParticleFactory particleFactory)
    {
        _damageService = damageService;
        _spatialService = spatialService;
        _coroutineRunner = coroutineRunner;
        _particleFactory = particleFactory;
    }

    private void OnHit (IEntity entity)
    {
        if (Host is Ball ball)
        {
            float totalDamage = ExecuteExplosion (ball.Transform.position, _config.GetRadius(Level), _config.GetDamage(Level));
            _particleFactory.Create(_config.ParticleObject, ball.Transform.position);
            ball.AddDamage(totalDamage);
            ball.gameObject.SetActive(false);
            _coroutineRunner.StartCoroutine(RespawnDelay(ball));
        }
    }

    private IEnumerator RespawnDelay(Ball ball)
    {
        yield return _respawnDelay;
        ball.EventHandler.Return(ball);
    }

    private float ExecuteExplosion (Vector3 center, float radius, float damage)
    {
        IEnumerable<IEntity> targets = _spatialService.GetEntitiesInRadius(center, radius)
            .Where(entity => entity != Host);

        float totalDamage = 0;

        foreach (var target in targets)
        {
            totalDamage += _damageService.ApplyDamage(target, damage);
        }

        return totalDamage;
    }

    protected override IDisposable SubscribeInternal (IEntity host)
    {
        host.EventHandler.Reflected += OnHit;
        return new DisposableObject(() => host.EventHandler.Reflected -= OnHit);
    }
}
