using System;

namespace BigBalls.GameplayObjects
{
    public class PlayerExperienceModel : IPlayerExperience
    {
        const int BaseEXPForLevelUp = 10;
        private Stat _stat;

        float EXPForNextLevel = BaseEXPForLevelUp;

        float CurrentEXP = 0;

        public event Action<IPlayerExperience> ValueChanged;
        public event Action LevelUped;

        public void Init (Stat stat)
        {
            _stat = stat;
        }

        public void AddExperience (float value)
        {

            CurrentEXP += value;

            if (CurrentEXP >= EXPForNextLevel)
            {
                LevelUp();
            }

            ValueChanged?.Invoke(this);
        }

        private void LevelUp()
        {
            _stat.AddCurrentValue(1);
            EXPForNextLevel = BaseEXPForLevelUp * _stat.CurrentValue;
            LevelUped?.Invoke();
        }
    }
}