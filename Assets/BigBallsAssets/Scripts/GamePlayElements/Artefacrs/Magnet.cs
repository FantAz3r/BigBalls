using System;
using BigBalls.GameplayObjects;
using BigBalls.Services;
using UnityEngine;
using VContainer;

public class Magnet : EffectBehaviour
{
    private MagnetConfig _magnetConfig;
    private IPlayerProvider _playerProvider;
    private IEntityRepository _entityRepository;
    private Stat _collectRange;
    public Magnet (MagnetConfig config, int level) : base(config, level)
    {
        _magnetConfig = config;
    }

    [Inject]
    public void Construct (IPlayerProvider playerProvider, IEntityRepository entityRepository)
    {
        _playerProvider = playerProvider;
        _entityRepository = entityRepository;
    }

    public override void OnUpgrade (int level)
    {
        base.OnUpgrade(level);
        _collectRange.ReduceCurrentValue(_collectRange.CurrentValue);
        _collectRange.AddCurrentValue(_magnetConfig.GetRange(Level));
    }

    protected override IDisposable SubscribeInternal (IEntity host)
    {
        _entityRepository.GetStatHolder(_playerProvider.Player).TryGetStat(out Stat stat, StatType.CollectRange);
        _collectRange = stat;
        _collectRange.ReduceCurrentValue(_collectRange.CurrentValue);
        _collectRange.AddCurrentValue(_magnetConfig.GetRange(Level));
        return new DisposableObject(() => { });
    }

    protected override void OnUnsubscribe (IEntity host = null)
    {
        _collectRange.ReduceCurrentValue(0);
        _collectRange.AddCurrentValue(_collectRange.StartCurrentValue);
    }
}
