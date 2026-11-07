using System;
using BigBalls.Configs;
using BigBalls.GameplayObjects;
using VContainer;

public class Poison : EffectBehaviour
{
    private readonly FireConfig _config;
    private DebuffService _debuffService;

    public Poison (FireConfig config, int level) : base(config, level) => _config = config;

    [Inject]
    public void Construct (DebuffService debuffService) => _debuffService = debuffService;

    private void OnHit (IEntity entity)
    {
        _debuffService.ApplyDamageOverTime(
            EffectType.Poison,
            entity,
            _config.GetBurnDPS(Level),
            _config.BurnDuration,
            _config.Color);
    }

    protected override IDisposable SubscribeInternal (IEntity host)
    {
        host.EventHandler.HitedEntity += OnHit;
        return new DisposableObject(() => host.EventHandler.HitedEntity -= OnHit);
    }
}
