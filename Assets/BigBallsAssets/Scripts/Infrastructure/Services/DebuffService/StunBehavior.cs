using BigBalls.GameplayObjects;

public sealed class StunBehavior : IDebuffBehavior
{
    public EffectType Type => EffectType.Stun;

    public void Apply (StatHolder holder, DebuffHandle handle, float value)
    {
        Zero(holder, handle, StatType.MoveSpeed);
        Zero(holder, handle, StatType.AttackSpeed);
        Zero(holder, handle, StatType.RotationSpeed);
    }

    private static void Zero (StatHolder holder, DebuffHandle handle, StatType type)
    {
        if (holder.TryGetStat(out Stat stat, type))
            handle.Remember(stat, stat.ReduceCurrentValue(stat.CurrentValue));
    }
}