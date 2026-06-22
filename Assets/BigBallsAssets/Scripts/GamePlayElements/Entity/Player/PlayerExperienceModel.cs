using System;

namespace BigBalls.GameplayObjects
{
    public class PlayerExperienceModel : IPlayerExperience
    {
        private const float UpgradeMultipler = 1.2f;

        public Stat Stat { get; private set; }
        public float CurrentEXP { get; private set; } = 0;
        public float EXPForNextLevel { get; private set; } = 100;


        public event Action ValueChanged;
        public event Action LevelUpped;

        public void Init (Stat stat)
        {
            EXPForNextLevel = stat.MaxValue;
        }

        public void AddExperience (float value)
        {
            CurrentEXP += value;

            if (Stat.CurrentValue >= EXPForNextLevel)
            {
                LevelUp();
            }

            ValueChanged?.Invoke();
        }

        private void LevelUp ()
        {
            Stat.AddCurrentValue(1);
            LevelUpped?.Invoke();
            EXPForNextLevel = EXPForNextLevel * UpgradeMultipler;
            CurrentEXP = 0;
        }
    }
}