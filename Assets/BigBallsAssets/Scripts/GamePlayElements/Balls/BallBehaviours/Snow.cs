using System;
using BigBalls.GameplayObjects;
using UnityEngine;
using VContainer;

public class Snow : EffectBehaviour
{
    private readonly SnowConfig _config;
    private DebuffService _debuffService;

    public Snow (SnowConfig config, int level) : base(config, level)
    {
        _config = config;
    }

    [Inject]
    public void Construct (DebuffService debuffService)
    {
        _debuffService = debuffService;
    }

    protected override IDisposable SubscribeInternal (IEntity host)
    {
        host.EventHandler.HitedEntity += OnHit;
        return new DisposableObject(() => host.EventHandler.HitedEntity -= OnHit);
    }

    private void OnHit (IEntity entity)
    {
        if (entity is Player player)
        {
            _debuffService.Apply(
           EffectType.Freeze,
           player,
           _config.FreezeDuration,
           _config.SlowPercent);
        }
        else if (entity is Enemy enemy)
        {
            enemy.StatHolder.TryGetStat(out Stat shield, StatType.Shield);
            shield.AddCurrentValue(_config.ShieldToEnwmy);
        }
    }
}
