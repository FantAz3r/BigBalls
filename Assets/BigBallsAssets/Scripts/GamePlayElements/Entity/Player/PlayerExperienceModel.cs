using System;

namespace BigBalls.GameplayObjects
{
    public class PlayerExperienceModel : IPlayerExperience
    {
        public float CurrentValue { get; private set; }
        public float MaxValue { get; private set; }
        
        public event Action<IPlayerExperience> OnValueChanged;
        
        public void Init(Stat stat)
        {
            CurrentValue = stat.CurrentValue;
            MaxValue = stat.MaxValue;
        }

        public void AddExperience(float value)
        {
            CurrentValue += value;
            OnValueChanged?.Invoke(this);
        }
    }
}