using System;
using System.Collections;
using System.Collections.Generic;
using BigBalls.GameplayObjects;
using BigBalls.Services;
using UnityEngine;

public class DebuffService
{
    private readonly IEntityRepository _entityRepository;
    private readonly IDamageService _damageService;
    private readonly ICoroutineRunner _coroutineRunner;

    private readonly Dictionary<(IEntity, EffectType), DebuffHandle> _active = new();

    public DebuffService (IEntityRepository entityRepository, ICoroutineRunner coroutineRunner, IDamageService damageService)
    {
        _entityRepository = entityRepository;
        _damageService = damageService;
        _coroutineRunner = coroutineRunner;
    }

    public void Apply (EffectType type, IEntity entity, float duration, float value = 0, Action callback = null)
    {
        if (entity == null || duration <= 0f)
            return;

        var holder = _entityRepository.GetStatHolder(entity);
        if (holder == null)
            return;

        if (_active.TryGetValue((entity, type), out var existing))
            FinishHandle(existing, cancelled: true);

        var handle = new DebuffHandle(entity, type);
        _active[(entity, type)] = handle;

        switch (type)
        {
            case EffectType.Stun:
                ApplyStun(holder, handle);
                break;
            case EffectType.Freeze:
                ApplySlow(holder, handle, value);
                break;
            case EffectType.Disarm:
                ApplyDisarm(holder, handle);
                break;
            case EffectType.ArmorReduction:
                ApplyArmorReduction(holder, handle, value);
                break;
        }

        _coroutineRunner.StartCoroutine(RunTimer(handle, duration, callback));
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


        if (_active.TryGetValue((entity, type), out var handle) == false)
        {
            handle = new DebuffHandle(entity, type);
            _active[(entity, type)] = handle;
            handle.AddContribution(dps, color, duration, onFinished);
            _coroutineRunner.StartCoroutine(RunTickTimer(handle));
            return;
        }

        handle.AddContribution(dps, color, duration, onFinished);
    }

    public void Cancel (IEntity entity, EffectType type)
    {
        if (entity == null)
            return;

        if (_active.TryGetValue((entity, type), out var handle))
            FinishHandle(handle, cancelled: true);
    }

    private IEnumerator RunTickTimer (DebuffHandle handle)
    {
        while (handle.HasContributions && handle.Cancelled == false)
        {
            if (handle.Entity == null
                || handle.Entity.Transform == null
                || handle.Entity.Transform.gameObject.activeSelf == false)
                break;

            float dt = Time.deltaTime;

            foreach (var c in handle.Contributions)
            {
                if (c.Remaining <= 0f || c.Finished)
                    continue;

                c.Remaining -= dt;
                c.TickAccumulator += dt;

                while (c.TickAccumulator >= 1f && !c.Finished)
                {
                    c.TickAccumulator -= 1f;
                    _damageService.ApplyDamage(handle.Entity, c.Dps, c.Color);
                    c.AccumulatedDamage += c.Dps;

                    if (c.Remaining <= 0f)
                    {
                        c.Finish(cancelled: false);
                        break;
                    }
                }
            }

            handle.RemoveFinished();

            yield return null;
        }

        FinishHandle(handle, cancelled: handle.Cancelled);
    }

    private IEnumerator RunTimer (DebuffHandle handle, float duration, Action callback)
    {
        handle.Remaining = duration;

        while (handle.Remaining > 0f && !handle.Cancelled)
        {
            if (handle.Entity == null
                || handle.Entity.Transform == null
                || handle.Entity.Transform.gameObject.activeSelf == false)
                break;

            handle.Remaining -= Time.deltaTime;
            yield return null;
        }

        bool cancelled = handle.Cancelled;
        FinishHandle(handle, cancelled);
        callback?.Invoke();
    }

    private void FinishHandle (DebuffHandle handle, bool cancelled)
    {
        if (handle.Finished)
            return;

        handle.Finished = true;
        handle.Restore();

        foreach (var c in handle.Contributions)
            c.Finish(cancelled);

        handle.Contributions.Clear();

        var key = (handle.Entity, handle.Type);

        if (_active.TryGetValue(key, out var current) && current == handle)
            _active.Remove(key);
    }

    private void ApplyStun (StatHolder holder, DebuffHandle handle)
    {
        if (holder.TryGetStat(out Stat move, StatType.MoveSpeed))
            handle.Remember(move, move.ReduceCurrentValue(move.CurrentValue));

        if (holder.TryGetStat(out Stat attack, StatType.AttackSpeed))
            handle.Remember(attack, attack.ReduceCurrentValue(attack.CurrentValue));

        if (holder.TryGetStat(out Stat rotation, StatType.RotationSpeed))
            handle.Remember(rotation, rotation.ReduceCurrentValue(rotation.CurrentValue));
    }

    private void ApplySlow (StatHolder holder, DebuffHandle handle, float percent)
    {
        percent = Mathf.Clamp01(percent);

        if (holder.TryGetStat(out Stat move, StatType.MoveSpeed))
            handle.Remember(move, move.ReduceCurrentValue(move.CurrentValue * percent));

        if (holder.TryGetStat(out Stat attack, StatType.AttackSpeed))
            handle.Remember(attack, attack.ReduceCurrentValue(attack.CurrentValue * percent));

        if (holder.TryGetStat(out Stat rotation, StatType.RotationSpeed))
            handle.Remember(rotation, rotation.ReduceCurrentValue(rotation.CurrentValue * percent));
    }

    private void ApplyDisarm (StatHolder holder, DebuffHandle handle)
    {
        if (holder.TryGetStat(out Stat damage, StatType.AttackSpeed))
            handle.Remember(damage, damage.ReduceCurrentValue(damage.CurrentValue));
    }

    private void ApplyArmorReduction (StatHolder holder, DebuffHandle handle, float value)
    {
        if (holder.TryGetStat(out Stat armor, StatType.Armor))
            handle.Remember(armor, armor.ReduceCurrentValue(value));
    }


    private class DebuffHandle
    {
        private readonly List<(Stat stat, float removed)> _restore = new();

        public IEntity Entity { get; }
        public EffectType Type { get; }
        public bool Cancelled { get; private set; }
        public bool Finished { get; set; }
        public float Remaining { get; set; }

        public List<DotContribution> Contributions { get; } = new();
        public bool HasContributions => Contributions.Count > 0;

        public DebuffHandle (IEntity entity, EffectType type)
        {
            Entity = entity;
            Type = type;

            if (Entity is Enemy enemy && enemy.EffectViewer != null)
            {
                enemy.EffectViewer.EnableEffect(Type);
            }
        }

        public void AddContribution (float dps, Color color, float duration, Action<DebuffResult> onFinished)
        {
            var c = new DotContribution(dps, color, duration, onFinished);
            c.Bind(Entity, Type);
            Contributions.Add(c);
        }

        public void RemoveFinished ()
        {
            for (int i = Contributions.Count - 1; i >= 0; i--)
            {
                if (Contributions[i].Finished)
                    Contributions.RemoveAt(i);
            }
        }

        public void Remember (Stat stat, float removed)
        {
            if (removed > 0f)
                _restore.Add((stat, removed));
        }

        public void Cancel () => Cancelled = true;

        public void Restore ()
        {
            if (Entity is Enemy enemy && enemy.EffectViewer != null)
                enemy.EffectViewer.DisableEffect(Type);

            foreach (var (stat, removed) in _restore)
                stat.AddCurrentValue(removed);
            _restore.Clear();
        }
    }

    private class DotContribution
    {
        public float Dps;
        public Color Color;
        public float Remaining;
        public float TickAccumulator;
        public float AccumulatedDamage;
        public bool Finished;

        private Action<DebuffResult> _onFinished;
        private IEntity _entity;
        private EffectType _type;

        public DotContribution (float dps, Color color, float duration, Action<DebuffResult> onFinished)
        {
            Dps = dps;
            Color = color;
            Remaining = duration;
            _onFinished = onFinished;
        }

        public void Bind (IEntity entity, EffectType type)
        {
            _entity = entity;
            _type = type;
        }

        public void Finish (bool cancelled)
        {
            if (Finished)
                return;

            Finished = true;

            var cb = _onFinished;
            _onFinished = null;
            cb?.Invoke(new DebuffResult(_entity, _type, AccumulatedDamage, cancelled));
        }
    }
}