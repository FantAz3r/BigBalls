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

    public void Apply (EffectType type, IEntity entity, float duration, float slowPercent = 0.5f)
    {
        if (entity == null || duration <= 0f)
            return;

        StatHolder holder = _entityRepository.GetStatHolder(entity);
        if (holder == null)
            return;

        if (_active.TryGetValue((entity, type), out var existing))
        {
            existing.Cancel();
            _active.Remove((entity, type));
        }

        var handle = new DebuffHandle(entity, type);
        _active[(entity, type)] = handle;

        switch (type)
        {
            case EffectType.Stun:
                ApplyStun(holder, handle);
                break;
            case EffectType.Freeze:
                ApplySlow(holder, handle, slowPercent);
                break;
            case EffectType.Disarm:
                ApplyDisarm(holder, handle);
                break;
        }

        _coroutineRunner.StartCoroutine(RunTimer(handle, duration));
    }

    public void ApplyDamageOverTime (EffectType type, IEntity entity, float dps, float duration, Color color)
    {
        if (entity == null || duration <= 0f || dps <= 0f) return;

        if (_active.TryGetValue((entity, type), out var existing))
        {
            existing.AddDps(dps, color, duration);
            return;
        }

        var handle = new DebuffHandle(entity, type);
        handle.AddDps(dps, color, duration);
        _active[(entity, type)] = handle;

        _coroutineRunner.StartCoroutine(RunTickTimer(handle));
    }

    private IEnumerator RunTickTimer (DebuffHandle handle)
    {
        while (handle.Remaining > 0f && !handle.Cancelled)
        {
            if (handle.Entity == null || handle.Entity.Transform == null ||
                handle.Entity.Transform.gameObject.activeSelf == false)
                break;

            handle.Remaining -= Time.deltaTime;
            handle.TickAccumulator += Time.deltaTime;

            if (handle.TickAccumulator >= 1f)
            {
                handle.TickAccumulator -= 1f;
                _damageService.ApplyDamage(handle.Entity, handle.Dps, handle.TickColor);
            }

            yield return null;
        }

        _active.Remove((handle.Entity, handle.Type));
    }

    private IEnumerator RunTimer (DebuffHandle handle, float duration)
    {
        while (handle.Remaining > 0f && !handle.Cancelled)
        {
            if (handle.Entity == null ||
                handle.Entity.Transform == null ||
                handle.Entity.Transform.gameObject.activeSelf == false)
                break;

            if (handle.Cancelled)
                break;

            handle.Remaining -= Time.deltaTime;
            yield return null;
        }

        handle.Restore();
        _active.Remove((handle.Entity, handle.Type));
    }

    private void ApplyStun (StatHolder holder, DebuffHandle handle)
    {
        if (holder.TryGetStat(out Stat move, StatType.MoveSpeed))
            handle.Remember(move, move.ReduceCurrentValue(move.CurrentValue));

        if (holder.TryGetStat(out Stat attack, StatType.AttackSpeed))
            handle.Remember(attack, attack.ReduceCurrentValue(attack.CurrentValue));
    }

    private void ApplySlow (StatHolder holder, DebuffHandle handle, float percent)
    {
        percent = Mathf.Clamp01(percent);

        if (holder.TryGetStat(out Stat move, StatType.MoveSpeed))
        {
            float removed = move.ReduceCurrentValue(move.CurrentValue * percent);
            handle.Remember(move, removed);
        }

        if (holder.TryGetStat(out Stat attack, StatType.AttackSpeed))
        {
            float removed = attack.ReduceCurrentValue(attack.CurrentValue * percent);
            handle.Remember(attack, removed);
        }
    }

    private void ApplyDisarm (StatHolder holder, DebuffHandle handle)
    {
        if (holder.TryGetStat(out Stat damage, StatType.AttackSpeed))
            handle.Remember(damage, damage.ReduceCurrentValue(damage.CurrentValue));
    }

    private class DebuffHandle
    {
        private readonly List<(Stat stat, float removed)> _restore = new();

        public float Dps;
        public float TickAccumulator;
        public Color TickColor;
        public float Remaining;

        public void Refresh (float duration) => Remaining = Mathf.Max(Remaining, duration);
        public IEntity Entity { get; }
        public EffectType Type { get; }
        public bool Cancelled { get; private set; }

        public DebuffHandle (IEntity entity, EffectType type)
        {
            Entity = entity;
            Type = type;

            if (Entity is Enemy enemy)
                enemy.EffectViewer.EnableEffect(Type, -1);
        }

        public void AddDps (float added, Color color, float duration)
        {
            Dps += added;
            TickColor = color;
            Remaining = Mathf.Max(Remaining, duration);
        }

        public void Remember (Stat stat, float removed)
        {
            if (removed > 0f)
                _restore.Add((stat, removed));
        }

        public void Cancel () => Cancelled = true;

        public void Restore ()
        {
            if (Entity is Enemy enemy)
                enemy.EffectViewer.DisableEffect(Type);


            foreach (var (stat, removed) in _restore)
                stat.AddCurrentValue(removed);
            _restore.Clear();
        }
    }
}
