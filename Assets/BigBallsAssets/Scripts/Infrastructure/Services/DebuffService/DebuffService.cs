using System;
using System.Collections.Generic;
using BigBalls.GameplayObjects;
using BigBalls.Services;
using UnityEngine;

public sealed class DebuffService
{
    private readonly IEntityRepository _entityRepository;
    private readonly ICoroutineRunner _coroutineRunner;
    private readonly DotTicker _dotTicker;
    private readonly DurationTimer _durationTimer;
    private readonly DebuffRegistry _registry;
    private readonly IReadOnlyDictionary<EffectType, IDebuffBehavior> _behaviors;
    private readonly IDebuffBehavior[] _debuffBehaviors = new IDebuffBehavior[]
    {
        new StunBehavior(),
        new SlowBehavior(),
        new DisarmBehavior(),
        new ArmorReductionBehavior(),
        new BlindBehavior()
    };

    public DebuffService (
        IEntityRepository entityRepository,
        ICoroutineRunner coroutineRunner,
        DotTicker dotTicker,
        DurationTimer durationTimer,
        DebuffRegistry registry)
    {
        _entityRepository = entityRepository;
        _coroutineRunner = coroutineRunner;
        _dotTicker = dotTicker;
        _durationTimer = durationTimer;
        _registry = registry;

        var map = new Dictionary<EffectType, IDebuffBehavior>();

        foreach (var behaviour in _debuffBehaviors)
            map[behaviour.Type] = behaviour;

        _behaviors = map;
    }

    public void Apply (EffectType type, IEntity entity, float duration, float value = 0f, Action<bool> callback = null)
    {
        if (entity == null || duration <= 0f)
            return;

        if (entity is Enemy enemy)
        {
            if (ApplyResist(enemy, type, callback))
                return;
        }

        var holder = _entityRepository.GetStatHolder(entity);

        if (holder == null)
            return;

        if (_behaviors.TryGetValue(type, out var behavior) == false)
            return;

        if (_registry.TryGet(entity, type, out var existing))
            FinishHandle(existing, cancelled: true);

        var handle = new DebuffHandle(entity, type);
        _registry.Add(handle);

        behavior.Apply(holder, handle, value);

        _coroutineRunner.StartCoroutine(
            _durationTimer.Run(handle, duration, cancelled => OnDurationComplete(handle, cancelled, callback)));
    }

    public void ApplyDamageOverTime (
        EffectType type,
        IEntity entity,
        float dps,
        float duration,
        Color color,
        Action<DebuffResult> onFinished = null)
    {
        if (entity == null || duration <= 0f || dps <= 0f)
            return;

        if (entity is Enemy enemy)
            if (ApplyResist(enemy, type))
                return;

        if (_registry.TryGet(entity, type, out var handle))
        {
            handle.AddContribution(new DotContribution(dps, color, duration, entity, type, onFinished));
            return;
        }

        handle = new DebuffHandle(entity, type);
        _registry.Add(handle);
        handle.AddContribution(new DotContribution(dps, color, duration, entity, type, onFinished));

        _coroutineRunner.StartCoroutine(_dotTicker.Run(handle));
    }

    public void Cancel (IEntity entity, EffectType type)
    {
        if (entity == null)
            return;

        if (_registry.TryGet(entity, type, out var handle))
            FinishHandle(handle, cancelled: true);
    }

    private void OnDurationComplete (DebuffHandle handle, bool cancelled, Action<bool> callback)
    {
        FinishHandle(handle, cancelled);
        callback?.Invoke(cancelled);
    }

    private void FinishHandle (DebuffHandle handle, bool cancelled)
    {
        if (handle.Finished) return;
        handle.Finished = true;

        if (cancelled) handle.Cancel();
        handle.Restore();

        foreach (var c in handle.Contributions)
            c.Finish(cancelled);
        handle.Contributions.Clear();

        _registry.RemoveSame(handle);
    }

    private bool ApplyResist (Enemy enemy, EffectType type, Action<bool> callback = null)
    {
        foreach (var resist in enemy.Config.DebuffResists)
        {
            if (type == resist)
            {
                callback?.Invoke(true);
                return true;
            }
        }

        return false;
    }
}