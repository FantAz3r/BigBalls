using System;
using BigBalls.GameplayObjects;
using UnityEngine;
using VContainer;

public class Lava : EffectBehaviour
{
    private readonly LavaConfig _config;
    private DebuffService _debuffService;

    public Lava (LavaConfig config, int level) : base(config, level)
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
        host.EventHandler.HitedEntity += OnHited;

        if (host.TriggerCollider.TryGetComponent(out LavaTrigger lavaTrigger))
        {
            lavaTrigger.Initialize(OnEnemyEntered, _config.GetRadius(Level));
        }

        return new DisposableObject(() =>
        {
            host.EventHandler.HitedEntity -= OnHited;
        });
    }

    private void OnHited(IEntity entity)
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
                _config.Color
            );
        }
    }
}
