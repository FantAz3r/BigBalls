using BigBalls.GameplayObjects;
using BigBalls.Infrastructure;
using System.Collections.Generic;
using UnityEngine;

namespace BigBalls.Configs
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "Configs/LevelConfig")]

    public class LevelConfig : ScriptableObject
    {
        [field: SerializeField] public LevelID Level { get; private set; } = LevelID.Level1;
        [field: SerializeField] public List<Tile> TilePrefabs { get; private set; }
        [field: SerializeField] public List<Wave> Waves { get; private set; }

        [field: SerializeField] public float BossTimeDelay { get; private set; }
        [field: SerializeField] public EnemyConfig LevelBoss { get; private set; }
        public float LineSpawnDelay { get; private set; } = 5;

        public List<EnemyConfig> GetAllLevelEnemies()
        {
            List<EnemyConfig> enemyConfigs = new ();

            foreach (var wave in Waves)
            {
                enemyConfigs.AddRange(wave.Enemies);
            }

            foreach (var wave in Waves)
            {
                if (wave.BossConfig != null)
                    enemyConfigs.Add(wave.BossConfig);
            }

            enemyConfigs.Add(LevelBoss);

            return enemyConfigs;
        }

        public List<EnemyConfig> GetWaveEnemies(int waveIndex)
        {
            return Waves[waveIndex].Enemies;
        }

        public float GetWaveTime(int waveId)
        {
            float waveTime = 0;

            waveTime += Waves[waveId].LineCount * LineSpawnDelay + Waves[waveId].WaveCooldown;

            if (waveId == Waves.Count -1)
            {
                waveTime += BossTimeDelay;
            }

            return waveTime;
        }

        public float GetLevelTime()
        {
            float fullTime = 0;

            for (int i = 0; i < Waves.Count; i++)
            {
                fullTime += GetWaveTime(i);
            }

            fullTime += BossTimeDelay;

            return fullTime;
        }
    }
}