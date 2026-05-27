using System;

namespace BigBalls.GameplayObjects
{
    public interface IReadonlyStat
    {
        float MinValue { get; }
        float MaxValue { get; }
        float CurrentValue { get; }

        event Action<IReadonlyStat> ValueChanged;
    }
}