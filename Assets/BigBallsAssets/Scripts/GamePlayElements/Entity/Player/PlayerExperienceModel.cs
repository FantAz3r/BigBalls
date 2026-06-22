using System;

namespace BigBalls.GameplayObjects
{
    public class PlayerExperienceModel : IPlayerExperience
    {
        private const float BaseEXPForLevelUp = 1.2f;

        private float _eXPForNextLevel;
        private float _startValueEXP = 0;

        public float MaxValue { get; private set; }
        public float CurrentValue { get; private set; }

        public event Action<IPlayerExperience> ValueChanged;
        public event Action LevelUpped;

        public void Init(Stat stat)
        {
            MaxValue = stat.MaxValue;
            _eXPForNextLevel = stat.MaxValue;
            CurrentValue = _startValueEXP;
        }

        public void AddExperience(float value)
        {
            CurrentValue += value;

            if (CurrentValue >= _eXPForNextLevel)
            {
                LevelUp();
            }

            ValueChanged?.Invoke(this);
        }

        private void LevelUp()
        {
            MaxValue = BaseEXPForLevelUp * _eXPForNextLevel;
            _eXPForNextLevel = MaxValue;
            LevelUpped?.Invoke();
            CurrentValue = _startValueEXP;
        }
    }
}