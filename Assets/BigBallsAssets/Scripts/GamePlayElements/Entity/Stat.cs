using System;

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

        public float MinValue { get; private set; }
        public float MaxValue { get; private set; }
        public float CurrentValue { get; private set; }

        public void AddCurrentValue(float valueToAdd)
        {
            if (valueToAdd <= 0) return;

            CurrentValue = MathF.Min(CurrentValue + valueToAdd, MaxValue);
            ValueChanged?.Invoke(this);
        }

        public void ReduceCurrentValue(float valueToResuce)
        {
            if (valueToResuce <= 0) return;

            CurrentValue = MathF.Max(CurrentValue - valueToResuce, MinValue);
            ValueChanged?.Invoke(this);
        }

        public void AddMaxValue(float valueToAdd)
        {
            if (valueToAdd <= 0) return;

            float percent = (MaxValue > 0) ? CurrentValue / MaxValue : 0f;
            MaxValue += valueToAdd;
            CurrentValue = percent * MaxValue;
            ValueChanged?.Invoke(this);
        }

        public void ReduceMaxStat(float valueToResuce)
        {
            if (valueToResuce <= 0) return;

            float percent = (MaxValue > 0) ? CurrentValue / MaxValue : 0f;
            MaxValue = MathF.Max(MinValue + 1, MaxValue - valueToResuce);
            CurrentValue = percent * MaxValue;
            ValueChanged?.Invoke(this);
        }
    }
}