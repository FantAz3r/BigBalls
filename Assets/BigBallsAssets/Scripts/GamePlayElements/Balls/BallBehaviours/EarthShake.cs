using BigBalls.Factories;
using BigBalls.GameplayObjects;
using BigBalls.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;

public class EarthShake : EffectBehaviour
{
    private readonly GroundBallConfig _config;
    private IDamageService _damageService;
    private ISpatialService _spatialService;
    private ICoroutineRunner _coroutineRunner;
    private IParticleFactory _particleFactory;

    private WaitForSeconds _oneSecond;

    public EarthShake(GroundBallConfig config, int level) : base(config, level)
    {
        _config = config;
        _oneSecond = new WaitForSeconds(1);
    }

    [Inject]
    public void Construct(IDamageService damageService, ISpatialService spatialService, ICoroutineRunner coroutineRunner, IParticleFactory particleFactory)
    {
        _damageService = damageService;
        _spatialService = spatialService;
        _coroutineRunner = coroutineRunner;
        _particleFactory = particleFactory;
    }

    public void OnHit(IEntity entity)
    {
        if (entity is Ball ball)
        {
            _coroutineRunner.StartCoroutine(ShakeRoutine(ball));
        }
    }

    private IEnumerator ShakeRoutine(Ball ball)
    {
        float elapsed = 0;
        float totalDamage = 0;
        Vector3 position = ball.Transform.position;

        while (_config.Duration > elapsed)
        {
            ParticleObject particleObject = _particleFactory.Create(_config.EarthShakeParticle, position);
            particleObject.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
            totalDamage += ApplyAreaDamage(position, _config.Radius, _config.GetDamage(Level));
            yield return _oneSecond;
            elapsed ++;
        }

        ball.AddDamage(totalDamage);
    }

    private float ApplyAreaDamage(Vector3 position, float radius, float damage)
    {
        IEnumerable<IEntity> targets = _spatialService.GetEntitiesInRadius(position, radius)
            .Where(entity => entity != Host);

        float totalDamage = 0;

        foreach (var target in targets)
        {
            totalDamage += _damageService.ApplyDamage(target, damage);
        }

        return totalDamage;
    }

    protected override IDisposable SubscribeInternal(IEntity host)
    {
        host.EventHandler.Reflected += OnHit;
        return new DisposableObject(() => host.EventHandler.Reflected -= OnHit);
    }
}
