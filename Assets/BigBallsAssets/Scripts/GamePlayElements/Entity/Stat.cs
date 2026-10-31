using System;

namespace BigBalls.GameplayObjects
{
    public class Stat : IReadonlyStat
    {
        public readonly StatType Type;

        public Stat (StatType statType, float startMaxValue, float minValue, float startCurrentValue)
        {
            if (startCurrentValue < minValue)
                throw new InvalidOperationException(nameof(startCurrentValue));

            if (startMaxValue < minValue)
                throw new InvalidOperationException(nameof(startMaxValue));

            Type = statType;

            MinValue = minValue;
            StartMaxValue = startMaxValue;
            StartCurrentValue = MathF.Min(startCurrentValue, StartMaxValue);
            CurrentValue = StartCurrentValue;
            MaxValue = StartMaxValue;
        }

        public event Action<IReadonlyStat> ValueChanged;
        public event Action<IReadonlyStat> ResetedToMiValue;
        public event Action ValueRedused;
        public event Action Immuned;
        public event Action Unimmuned;
        public event Action ValueIncreased;

        public bool CanReduce { get; private set; } = true;
        public bool CanExceedMaxValue { get; private set; }

        public float StartMaxValue { get; private set; }
        public float StartCurrentValue { get; private set; }
        public float StartMinValue { get; private set; }

        public float MinValue { get; private set; }
        public float MaxValue { get; private set; }
        public float CurrentValue { get; private set; }

        public float AddCurrentValue (float valueToAdd)
        {
            if (valueToAdd <= 0)
                return 0;

            float before = CurrentValue;

            if (CanExceedMaxValue)
                CurrentValue += valueToAdd;
            else
                CurrentValue = MathF.Min(CurrentValue + valueToAdd, MaxValue);
            float actualAdd = CurrentValue - before;

            ValueChanged?.Invoke(this);
            ValueIncreased?.Invoke();
            return actualAdd;
        }

        public float ReduceCurrentValue (float valueToResource)
        {
            if (CanReduce == false)
                return 0;

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

        public void SetCurrentValue(float value)
        {
            if (value <= MinValue)
                CurrentValue = MinValue + 0.1f;

            if (value > MaxValue)
                CurrentValue = MaxValue;

            CurrentValue = value;
        }


        public void ResetStat()
        {
            MaxValue = StartMaxValue;
            MinValue = StartMinValue;
            CurrentValue = StartCurrentValue;
        }

        public float SelfReduceCurrentValue (float valueToResource)
        {
            if (CanReduce == false)
                return 0;

            if (valueToResource <= 0)
                return 0;

            float before = CurrentValue;
            CurrentValue = MathF.Max(CurrentValue - valueToResource, 1);
            float actualReduced = before - CurrentValue;

            ValueChanged?.Invoke(this);
            return actualReduced;
        }

        public void AddMaxValue (float valueToAdd)
        {
            if (valueToAdd <= 0)
                return;

            float percent = (MaxValue > 0) ? CurrentValue / MaxValue : 0f;
            MaxValue += valueToAdd;
            CurrentValue = percent * MaxValue;
            ValueChanged?.Invoke(this);
        }

        public void SetMaxValue (float value)
        {
            if (value <= MinValue)
                MaxValue = MinValue + 0.1f;

            MaxValue = value;
        }

        public void ReduceMaxStat (float valueToResuce)
        {
            if (valueToResuce <= 0)
                return;

            float percent = (MaxValue > 0) ? CurrentValue / MaxValue : 0f;
            MaxValue = MathF.Max(MinValue + 1, MaxValue - valueToResuce);
            CurrentValue = percent * MaxValue;
            ValueChanged?.Invoke(this);
        }

        public void SetImmune (bool isImmune)
        {
            CanReduce = isImmune == false;

            if (isImmune)
            {
                Immuned?.Invoke();
            }
            else
            {
                Unimmuned?.Invoke();
            }
        }

        public void SetExceedMax (bool canExceedMaxValue) => CanExceedMaxValue = canExceedMaxValue;
    }
}