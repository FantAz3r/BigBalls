using System;
using System.Collections.Generic;
using BigBalls.GameplayObjects;
using BigBalls.Services;
using UnityEngine;
using VContainer;

public class Tunder : EffectBehaviour
{
    private readonly TunderConfig _config;

    private IPoolService _poolService;
    private IDamageService _damageService;

    public Tunder (TunderConfig config, int level) : base(config, level)
    {
        _config = config;
    }


    [Inject]
    public void Construct (IDamageService damageService, IPoolService poolService)
    {
        _poolService = poolService;
        _damageService = damageService;
    }

    public void ExecuteChain (IEntity startTarget)
    {
        if (Host is not Ball ball)
            return;

        int maxJumps = (int) _config.GetHitCount(Level);
        float searchRadius = _config.SearchRange;

        HashSet<IEntity> hitEntities = new HashSet<IEntity>();
        IEntity currentTarget = startTarget;

        for (int i = 0; i < maxJumps; i++)
        {
            if (currentTarget == null || hitEntities.Contains(currentTarget))
                break;

            float damage = _config.GetDamage(Level);
            _damageService.ApplyDamage(currentTarget, damage, _config.Color);
            ball.AddDamage(damage);

            hitEntities.Add(currentTarget);

            currentTarget = FindNextTarget(currentTarget.Transform.position, searchRadius, hitEntities);
        }

        LightningBolt lightningBolt = _poolService.GetObject<LightningBolt>("LigtningBoltPool");
        List<Transform> enemies = new();

        foreach (var entity in hitEntities)
        {
            enemies.Add(entity.Transform);
        }
        lightningBolt.LightFinished += OnRelease;
        lightningBolt.StartLightning(enemies, maxJumps);
    }

    private IEntity FindNextTarget (Vector3 position, float radius, HashSet<IEntity> excluded)
    {
        Collider[] colliders = Physics.OverlapSphere(position, radius);

        Enemy bestTarget = null;
        float minDistance = float.MaxValue;

        foreach (var collider in colliders)
        {
            if (collider.gameObject.TryGetComponent(out Enemy enemy) && excluded.Contains(enemy) == false)
            {
                float dist = Vector3.SqrMagnitude(position - enemy.transform.position);

                if (dist < minDistance)
                {
                    minDistance = dist;
                    bestTarget = enemy;
                }
            }
        }

        return bestTarget;
    }

    public void OnRelease(LightningBolt lightningBolt)
    {
        lightningBolt.LightFinished -= OnRelease;
        _poolService.ReleaseObject(lightningBolt);
    }

    protected override IDisposable SubscribeInternal (IEntity host)
    {
        host.EventHandler.HitedEntity += OnHit;
        return new DisposableObject(() => host.EventHandler.HitedEntity -= OnHit);
    }

    private void OnHit (IEntity entity)
    {
        ExecuteChain(entity);
    }
}
