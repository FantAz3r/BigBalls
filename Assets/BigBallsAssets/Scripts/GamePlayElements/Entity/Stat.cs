using System;
using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public class Stat : IReadonlyStat
    {
        public readonly StatType Type;
        private readonly float _startMaxValue;

        public Stat(StatType statType, float startMaxValue, float minValue, float startCurrentValue)
        {
            if (startCurrentValue > startMaxValue && startCurrentValue < minValue)
                throw new InvalidOperationException(nameof(startCurrentValue));

            if (startMaxValue < minValue)
                throw new InvalidOperationException(nameof(startMaxValue));

            Type = statType;

            _startMaxValue = startMaxValue;
            MinValue = minValue;
            CurrentValue = startCurrentValue;

            MaxValue = _startMaxValue;
        }

        public event Action<IReadonlyStat> ValueChanged;
        public event Action<IReadonlyStat> ResetedToMiValue;
        public event Action ValueRedused;
        public event Action ValueIncreased;

        public float MinValue { get; private set; }
        public float MaxValue { get; private set; }
        public float CurrentValue { get; private set; }

        public float AddCurrentValue (float valueToAdd)
        {
            if (valueToAdd <= 0)
                return 0;

            float before = CurrentValue;
            CurrentValue = MathF.Min(CurrentValue + valueToAdd, MaxValue);
            float actualAdd = CurrentValue - before;

            ValueChanged?.Invoke(this);
            ValueIncreased?.Invoke();

            return actualAdd;
        }

        public float ReduceCurrentValue(float valueToResource)
        {
            if (valueToResource <= 0)
                return 0;

            float before = CurrentValue;
            CurrentValue = MathF.Max(CurrentValue - valueToResource, MinValue);
            float actualReduced = before - CurrentValue;

            ValueRedused?.Invoke();
            ValueChanged?.Invoke(this);

            if (CurrentValue == MinValue)
                ResetedToMiValue?.Invoke(this);

            return actualReduced;
        }

        public void AddMaxValue(float valueToAdd)
        {
            if (valueToAdd <= 0)
                return;

            float percent = (MaxValue > 0) ? CurrentValue / MaxValue : 0f;
            MaxValue += valueToAdd;
            CurrentValue = percent * MaxValue;
            ValueChanged?.Invoke(this);
        }

        public void ReduceMaxStat(float valueToResuce)
        {
            if (valueToResuce <= 0)
                return;

            float percent = (MaxValue > 0) ? CurrentValue / MaxValue : 0f;
            MaxValue = MathF.Max(MinValue + 1, MaxValue - valueToResuce);
            CurrentValue = percent * MaxValue;
            ValueChanged?.Invoke(this);
        }
    }
}