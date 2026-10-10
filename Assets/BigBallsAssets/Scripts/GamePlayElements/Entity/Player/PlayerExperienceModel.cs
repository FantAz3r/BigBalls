using System;
using System.Collections;
using BigBalls.Services;
using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public class PlayerExperienceModel : IPlayerExperience
    {
        private const float UpgradeMultiplier = 1.2f;
        private readonly SoundSpawner _soundSpawner;
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly IPlayerProvider _playerProvider;
        private float _upgrageTime = 1f;
        private WaitForSeconds _wait;

        public Stat Stat { get; private set; }
        public float CurrentEXP { get; private set; } = 0;
        public float EXPForNextLevel { get; private set; }

        public event Action ValueChanged;
        public event Action LevelUpped;

        public PlayerExperienceModel (SoundSpawner soundSpawner, ICoroutineRunner coroutineRunner, IPlayerProvider playerProvider)
        {
            _soundSpawner = soundSpawner;
            _wait = new WaitForSeconds(_upgrageTime);
            _coroutineRunner = coroutineRunner;
            _playerProvider = playerProvider;
        }

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
            _soundSpawner.Spawn(SoundType.LevelUP);
            _playerProvider.Player.EffectViewer.EnableEffect(EffectType.Upgrade, _upgrageTime);
            _coroutineRunner.StartCoroutine(LevelUPRoutine());
            EXPForNextLevel = Mathf.Round(EXPForNextLevel * UpgradeMultiplier);
            CurrentEXP = 0;
        }

        private IEnumerator LevelUPRoutine ()
        {
            yield return _wait;
            Stat.AddCurrentValue(1);
            LevelUpped?.Invoke();
        }
    }
}