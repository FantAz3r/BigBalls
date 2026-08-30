using BigBalls.Factories;
using BigBalls.GameplayObjects;
using BigBalls.Services;
using System;
using UnityEngine;
using VContainer;

public class LineLaser : EffectBehaviour
{
    private readonly LaserConfig _config;
    private IParticleFactory _particleFactory;
    private IDamageService _damageService;

    public LineLaser(LaserConfig config, int level) : base(config, level)
    {
        _config = config;
    }

    [Inject]
    public void Construct(IParticleFactory particleFactory, IDamageService damageService)
    {
        _particleFactory = particleFactory;
        _damageService = damageService;
    }

    public void OnHit(IEntity entity)
    {
        if (entity is not Enemy enemy)
            return;

        Vector3? targetBlockCenter = GetClosestBlockCenter(enemy, Host.Transform.position);

        if (targetBlockCenter == null)
            return;

        Vector3 laserOrigin = targetBlockCenter.Value;
        float damage = _config.GetDamage(Level);

        Vector3 baseDirection = Vector3.forward;

        baseDirection.y = 0;
        baseDirection.Normalize();
        float baseAngle = Mathf.Atan2(baseDirection.x, baseDirection.z) * Mathf.Rad2Deg;

        float angleStep = _config.RayCount > 1
            ? _config.SpreadAngle / (_config.RayCount - 1)
            : 0;

        float startAngle = baseAngle - (_config.SpreadAngle / 2f) + _config.Offset;

        for (int i = 0; i < _config.RayCount; i++)
        {
            float currentAngleDeg = startAngle + (angleStep * i);

            float rad = currentAngleDeg * Mathf.Deg2Rad;
            Vector3 direction = new Vector3(Mathf.Sin(rad), 0, Mathf.Cos(rad));

            ExecuteRaycast(laserOrigin, direction, damage);
        }
    }

    private void ExecuteRaycast(Vector3 origin, Vector3 direction, float damage)
    {
        ParticleObject particle = _particleFactory.Create(_config.Particle, origin);
        particle.transform.rotation = Quaternion.LookRotation(-direction);
        particle.Play();

        // 4. Физика (используем direction как направление)
        RaycastHit[] hits = Physics.RaycastAll(origin, direction, _config.MaxRange);
        Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));

        Debug.DrawRay(origin, direction * _config.MaxRange, Color.red, 2f);

        foreach (var hit in hits)
        {
            if (hit.collider.TryGetComponent(out Enemy enemy))
            {
                _damageService.ApplyDamage(enemy, damage, _config.Color);
            }
        }
    }


    private Vector3? GetClosestBlockCenter(Enemy enemy, Vector3 hitPoint)
    {
        Vector3 enemyPos = enemy.Transform.position;
        Vector3 closestCenter = Vector3.zero;
        float minDistance = float.MaxValue;
        bool found = false;

        foreach (var posInt in enemy.Config.BlocksPositions)
        {
            Vector3 blockCenter = enemyPos + new Vector3(posInt.x, posInt.y, 0);

            float dist = Vector3.Distance(hitPoint, blockCenter);
            if (dist < minDistance)
            {
                minDistance = dist;
                closestCenter = blockCenter;
                found = true;
            }
        }

        return found ? closestCenter : (Vector3?)null;
    }

    protected override IDisposable SubscribeInternal(IEntity host)
    {
        host.EventHandler.HitedEntity += OnHit;
        return new DisposableObject(() => host.EventHandler.HitedEntity -= OnHit);
    }
}
