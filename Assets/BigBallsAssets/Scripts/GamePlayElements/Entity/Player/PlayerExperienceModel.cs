using System;
using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public class PlayerExperienceModel : IPlayerExperience
    {
        private const float UpgradeMultiplier = 1.2f;

        public Stat Stat { get; private set; }
        public float CurrentEXP { get; private set; } = 0;
        public float EXPForNextLevel { get; private set; }

        public event Action ValueChanged;
        public event Action LevelUpped;

        public void Init (Stat stat)
        {
            Stat = stat;
            EXPForNextLevel = Stat.MaxValue;
        }

        public void AddExperience (float value)
        {
            CurrentEXP += value;

            if (CurrentEXP >= EXPForNextLevel)
            {
                LevelUp();
            }

            ValueChanged?.Invoke();
        }

        private void LevelUp ()
        {
            Stat.AddCurrentValue(1);
            LevelUpped?.Invoke();
            EXPForNextLevel = Mathf.Round(EXPForNextLevel * UpgradeMultiplier);
            CurrentEXP = 0;
        }
    }
}