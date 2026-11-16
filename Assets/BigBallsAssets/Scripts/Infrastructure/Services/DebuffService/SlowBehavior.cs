using BigBalls.GameplayObjects;
using UnityEngine;

public sealed class SlowBehavior : IDebuffBehavior
{
    public EffectType Type => EffectType.Freeze;

    public void Apply (StatHolder holder, DebuffHandle handle, float percent)
    {
        percent = Mathf.Clamp01(percent);
        Reduce(holder, handle, StatType.MoveSpeed, percent);
        Reduce(holder, handle, StatType.AttackSpeed, percent);
        Reduce(holder, handle, StatType.RotationSpeed, percent);
    }

    private static void Reduce (StatHolder holder, DebuffHandle handle, StatType type, float percent)
    {
        if (holder.TryGetStat(out Stat stat, type))
            handle.Remember(stat, stat.ReduceCurrentValue(stat.CurrentValue * percent));
    }
}