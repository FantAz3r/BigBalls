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

        if (statHolder == null)
            yield break;

        Stat movespeed = statHolder[StatType.MoveSpeed];

        float slowAttackSpeedValue = 0;
        float slowRotationSpeedValue = 0;
        float armorReductionValue = 0;

        float slowValue = movespeed.CurrentValue * _config.GetSlowPercent(Level);
        movespeed.ReduceCurrentValue(slowValue);

        if (statHolder.TryGetStat(out Stat armor, StatType.Armor))
        {
            armorReductionValue = armor.ReduceCurrentValue(_config.GetArmorReduction(Level));
        }

        if (statHolder.TryGetStat(out Stat attackSpeed, StatType.AttackSpeed))
        {
            slowAttackSpeedValue = attackSpeed.CurrentValue * _config.GetSlowPercent(Level);
            attackSpeed.ReduceCurrentValue(slowValue);
        }

        if (statHolder.TryGetStat(out Stat rotationSpeed, StatType.RotationSpeed))
        {
            slowRotationSpeedValue = movespeed.CurrentValue * _config.GetSlowPercent(Level);
            rotationSpeed.ReduceCurrentValue(slowValue);
        }

        while (elapsed <= time)
        {
            if (entity == null && entity.Transform.gameObject.activeSelf == false)
                yield break;

            elapsed += Time.deltaTime;
            yield return null;
        }

        enemy.EnemyIdleBob.Play();
        movespeed.AddCurrentValue(slowValue);

        if (rotationSpeed != null)
            rotationSpeed.AddCurrentValue(slowRotationSpeedValue);

        if (attackSpeed != null)
            attackSpeed.AddCurrentValue(slowAttackSpeedValue);

        if (armor != null)
            armor.AddCurrentValue(armorReductionValue);
    }


    protected override IDisposable SubscribeInternal (IEntity host)
    {
        host.EventHandler.HitedEntity += OnHit;
        return new DisposableObject(() => host.EventHandler.HitedEntity -= OnHit);
    }
}
