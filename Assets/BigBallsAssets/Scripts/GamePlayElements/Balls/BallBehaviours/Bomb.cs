using System;
using System.Collections.Generic;
using System.Linq;
using BigBalls.GameplayObjects;
using BigBalls.Services;
using UnityEngine;
using VContainer;

public class Bomb : EffectBehaviour
{

    private readonly BombConfig _config;
    private IDamageService _damageService;
    private ISpatialService _spatialService;

    public Bomb (BombConfig config, int level) : base(config, level)
    {
        _config = config;
    }

    [Inject]
    public void Construct (IDamageService damageService, ISpatialService spatialService)
    {
        _damageService = damageService;
        _spatialService = spatialService;
    }

    private void OnHit (IEntity entity)
    {
        if (Host is Ball ball)
        {
            float totalDamage = ExecuteExplosion (ball.Transform.position, _config.GetRadius(Level), _config.GetDamage(Level));
            ball.AddDamage(totalDamage);
            ball.EventHandler.Return(ball);
        }
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
        host.EventHandler.HitedEntity += OnHit;
        return new DisposableObject(() => host.EventHandler.HitedEntity -= OnHit);
    }
}
