using BigBalls.GameplayObjects;

public sealed class ArmorReductionBehavior : IDebuffBehavior
{
    public EffectType Type => EffectType.ArmorReduction;

    public void Apply (StatHolder holder, DebuffHandle handle, float absolute)
    {
        if (holder.TryGetStat(out Stat armor, StatType.Armor))
            handle.Remember(armor, armor.ReduceCurrentValue(absolute));
    }
}