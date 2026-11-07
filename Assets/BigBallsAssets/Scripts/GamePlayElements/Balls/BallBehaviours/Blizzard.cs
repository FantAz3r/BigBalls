using System;
using BigBalls.GameplayObjects;
using BigBalls.Services;
using UnityEngine;
using VContainer;

public class Blizzard : EffectBehaviour
{
    private readonly BlizzardConfig _config;
    private DebuffService _debuffService;

    public Blizzard (BlizzardConfig config, int level) : base(config, level)
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
        if (host.TriggerCollider.TryGetComponent(out AuraTrigger trigger))
        {
            trigger.Initialize(OnEnemyEntered, _config.GetRadius(Level));
        }

        return new DisposableObject(() => { });
    }

    private void OnEnemyEntered (Collider other)
    {
        if (other.TryGetComponent(out Enemy enemy) == false)
            return;

        enemy.ColorChanger.SwapColor(_config.FreezeColor, _config.GetFreezeDuration(Level), 2);
        enemy.EnemyIdleBob?.Stop();

        _debuffService.Apply(
            EffectType.Freeze,
            enemy,
            _config.GetFreezeDuration(Level),
            0,
            () => enemy.EnemyIdleBob?.Play());
    }
}
