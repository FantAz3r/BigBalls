using System;
using BigBalls.GameplayObjects;
using BigBalls.StaticData;
using UnityEngine;
using VContainer;

public class Horn : EffectBehaviour
{
    private HornConfig _hornConfig;
    private IEntityRepository _entityRepository;

    public Horn (HornConfig config, int level) : base(config, level)
    {
        _hornConfig = config;
    }

    [Inject]
    public void Construct (IEntityRepository entityRepository)
    {
        _entityRepository = entityRepository;
    }


    protected override IDisposable SubscribeInternal (IEntity host)
    {
        host.EventHandler.Spawned += OnSpawn;
        return new DisposableObject(() => { });
    }


    protected override void OnUnsubscribe (IEntity host = null)
    {
        host.EventHandler.Spawned -= OnSpawn;
    }

    private void OnSpawn (IEntity host)
    {
        if (host is Ball ball && ball.Config.BallType == BallType.Base)
        {
            _entityRepository.GetStatHolder(ball).TryGetStat(out Stat stat, StatType.Damage);
            stat.AddCurrentValue(stat.CurrentValue * _hornConfig.GetDamage(Level));
        }
    }
}
