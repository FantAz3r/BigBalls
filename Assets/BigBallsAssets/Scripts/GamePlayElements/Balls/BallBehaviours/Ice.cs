using System;
using System.Collections;
using BigBalls.GameplayObjects;
using BigBalls.Services;
using BigBalls.StaticData;
using UnityEngine;
using VContainer;

public class Ice : EffectBehaviour
{
    private readonly IceConfig _config;
    private ICoroutineRunner _coroutineRunner;
    private IEntityRepository _entityRepository;

    private WaitForSeconds _freezeTime;

    public Ice (IceConfig config, int level) : base(config, level)
    {
        _config = config;
    }

    [Inject]
    public void Construct (ICoroutineRunner coroutineRunner, IEntityRepository entityRepository)
    {
        _coroutineRunner = coroutineRunner;
        _entityRepository = entityRepository;
    }

    public void OnHit (IEntity entity)
    {

        _freezeTime = new WaitForSeconds(_config.GetFreezeDuration(Level));
        _coroutineRunner.StartCoroutine(FreezeRoutine(entity));
    }

    private IEnumerator FreezeRoutine (IEntity entity)
    {
        if (entity is not Enemy enemy)
            yield break;

        float time = _config.GetFreezeDuration(Level);
        var elapsed = 0f;

        enemy.ColorChanger.SwapColor(_config.FreezeColor, time, 1);
        enemy.EffectViewer.EnableEffect(EffectType.Freeze);
        enemy.EnemyIdleBob.Stop();

        StatHolder statHolder = _entityRepository.GetStatHolder(enemy);
        Stat stat = statHolder[StatType.MoveSpeed];
        float slowValue = stat.CurrentValue * _config.GetSlowPercent(Level);
        stat.ReduceCurrentValue(slowValue);

        while (elapsed <= time)
        {
            if (entity.Transform.gameObject.activeSelf == false)
                yield break;

            elapsed += Time.deltaTime;
            yield return null;
        }

        enemy.EnemyIdleBob.Play();
        stat.AddCurrentValue(slowValue);
    }


    protected override IDisposable SubscribeInternal (IEntity host)
    {
        host.EventHandler.HitedEntity += OnHit;
        return new DisposableObject(() => host.EventHandler.HitedEntity -= OnHit);
    }
}
