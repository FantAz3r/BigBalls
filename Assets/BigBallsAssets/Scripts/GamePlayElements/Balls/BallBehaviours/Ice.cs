using System;
using BigBalls.GameplayObjects;
using BigBalls.StaticData;
using VContainer;

public class Ice : EffectBehaviour
{
    private readonly IceConfig _config;
    private DebuffService _debuffService;

    public Ice (IceConfig config, int level) : base(config, level)
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
        if (entity is not Enemy enemy)
            return;

        enemy.ColorChanger.SwapColor(_config.FreezeColor, _config.GetFreezeDuration(Level), 2);
        enemy.EnemyIdleBob?.Stop();

        _debuffService.Apply(
            EffectType.Freeze,
            enemy,
            _config.GetFreezeDuration(Level),
            _config.GetSlowPercent(Level),
            () => enemy.EnemyIdleBob?.Play());

        _debuffService.Apply(
            EffectType.ArmorReduction,
            enemy,
            _config.GetFreezeDuration(Level),
            _config.GetArmorReduction(Level));
    }
}