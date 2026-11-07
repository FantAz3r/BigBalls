using System;
using BigBalls.GameplayObjects;
using UnityEngine;
using VContainer;

public class Lava : EffectBehaviour
{
    private readonly LavaConfig _config;
    private DebuffService _debuffService;
    private SoundSpawner _soundSpawner;
    public Lava (LavaConfig config, int level) : base(config, level)
    {
        _config = config;
    }

    [Inject]
    public void Construct (DebuffService debuffService, SoundSpawner soundSpawner)
    {
        _debuffService = debuffService;
        _soundSpawner = soundSpawner;
    }

    protected override IDisposable SubscribeInternal (IEntity host)
    {
        host.EventHandler.HitedEntity += OnHited;

        if (host.TriggerCollider.TryGetComponent(out AuraTrigger lavaTrigger))
        {
            lavaTrigger.Initialize(OnEnemyEntered, _config.GetRadius(Level));
        }

        return new DisposableObject(() =>
        {
            host.EventHandler.HitedEntity -= OnHited;
        });
    }

    private void OnHited (IEntity entity)
    {

    }

    private void OnEnemyEntered (Collider other)
    {
        if (other.TryGetComponent(out IEntity enemy) && enemy is Enemy)
        {
            _debuffService.ApplyDamageOverTime(
                EffectType.Burn,
                enemy,
                _config.GetBurnDPS(Level),
                _config.GetBurnDuration(Level),
                _config.Color,
                result =>
                {
                    Ball?.CalculateDamage(result.Damage);
                }
            );

            _soundSpawner.Spawn(_config.BurnSound);
        }
    }
}
