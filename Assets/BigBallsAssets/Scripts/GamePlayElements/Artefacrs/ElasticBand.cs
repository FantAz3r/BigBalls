using BigBalls.GameplayObjects;
using System;
using UnityEngine;
using VContainer;

public class ElasticBand : EffectBehaviour
{
    private ElasticBandConfig _config;
    private IEntityRepository _entityRepository;
    private StatHolder _statHolder;
    private Stat _moveStat;
    private float _startMoveSpeed;
    private float _maxMoveSpeed;
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
        if (_moveStat.CurrentValue >= _maxMoveSpeed)
        {
            _moveStat.ReduceCurrentValue(_moveStat.CurrentValue);
            _moveStat.AddCurrentValue(_maxMoveSpeed);
        }
        else
        {
            _moveStat.AddCurrentValue(_moveStat.CurrentValue * _config.AddMoveSpeedPerHit);
        }
    }

    public void OnSpawn(IEntity host)
    {
        _statHolder = _entityRepository.GetStatHolder(host);
        _moveStat = _statHolder[StatType.MoveSpeed];
        _startMoveSpeed = _moveStat.CurrentValue;
        _maxMoveSpeed = _startMoveSpeed * _config.MaxMoveSpeedMultiplyer;

        _moveStat.ReduceCurrentValue(_startMoveSpeed);
        _moveStat.AddCurrentValue(_startMoveSpeed * _config.StartBallMoveSpeed);
    }

    protected override IDisposable SubscribeInternal(IEntity host)
    {
        OnSpawn(host);
        host.EventHandler.Hited += OnHit;
        return new DisposableObject(() => host.EventHandler.Hited -= OnHit);
    }

    protected override void OnUnsubscribe()
    {
        _moveStat.ReduceCurrentValue(_moveStat.CurrentValue);
        _moveStat.AddCurrentValue(_startMoveSpeed);
    }
}
