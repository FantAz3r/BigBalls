using BigBalls.GameplayObjects;
using System;
using VContainer;

public class ElasticBand : EffectBehaviour
{
    private ElasticBandConfig _config;
    private IEntityRepository _entityRepository;

    public ElasticBand(ElasticBandConfig config, int level) : base(config, level)
    {
        _config = config;
    }

    [Inject]
    public void Construct(IEntityRepository entityRepository)
    {
        _entityRepository = entityRepository;
    }

    public void OnHit()
    {
        StatHolder statHolder = _entityRepository.GetStatHolder(Host);
        Stat moveStat = statHolder[StatType.MoveSpeed];
        moveStat.AddCurrentValue(moveStat.CurrentValue * _config.AddMoveSpeedPerHit);
    }

    protected override IDisposable SubscribeInternal(IEntity host)
    {
        host.EventHandler.Hited += OnHit;
        return new DisposableObject(() => host.EventHandler.Hited -= OnHit);
    }
}
