using BigBalls.GameplayObjects;

public sealed class DisarmBehavior : IDebuffBehavior
{
    public EffectType Type => EffectType.Disarm;

    public void Apply (StatHolder holder, DebuffHandle handle, float value)
    {
        if (holder.TryGetStat(out Stat attack, StatType.AttackSpeed))
            handle.Remember(attack, attack.ReduceCurrentValue(attack.CurrentValue));
    }
}