using System;

namespace BigBalls.GameplayObjects
{
    public class Stat
    {
        public readonly StatType Type;
        private readonly float _minValue;

        public Stat(StatType statType, float maxValue, float minValue)
        {
            Type = statType;
            MaxValue = maxValue;
            _minValue = minValue;
            CurrentValue = maxValue;
        }

        public event Action<float, float> ValueChanged;
        public float MaxValue { get; private set; }
        public float CurrentValue { get; private set; }

        public void ChangeCurrentStat(float newValue)
        {
            if (newValue < _minValue || newValue > MaxValue)
                throw new ArgumentOutOfRangeException();

            CurrentValue = newValue;
            ValueChanged?.Invoke(CurrentValue, MaxValue);
        }

        public void ChangeMaxStat(float newValue)
        {
            if (newValue <= _minValue)
                throw new ArgumentOutOfRangeException();

            float percent = (MaxValue > 0) ? CurrentValue / MaxValue : 0f; 
            MaxValue = newValue;
            CurrentValue = percent * MaxValue;
            ValueChanged?.Invoke(CurrentValue, MaxValue);
        }
    }
}