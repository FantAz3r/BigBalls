using BigBalls.GameplayObjects;
using UnityEngine;

public sealed class BlindBehavior : IDebuffBehavior
{
    public EffectType Type => EffectType.Blind;

    public void Apply (StatHolder holder, DebuffHandle handle, float percent)
    {
        percent = Mathf.Clamp01(percent);

        if (holder.TryGetStat(out Stat accuracy, StatType.Accuracy))
            handle.Remember(accuracy, accuracy.ReduceCurrentValue(accuracy.CurrentValue * percent));
    }
}