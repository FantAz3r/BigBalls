using BigBalls.GameplayObjects;
using BigBalls.Infrastructure;
using BigBalls.StaticData;
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
        [field: SerializeField] public List<EnemyConfig> Enemies { get; private set; }

        [field: SerializeField] public float BossTimeDelay { get; private set; }
        [field: SerializeField] public EnemyConfig LevelBoss { get; private set; }

        public List<EnemyConfig> GetEnemiesForPool()
        {
            List<EnemyConfig> enemyConfigs = Enemies;
            enemyConfigs.Add(LevelBoss);

            foreach(var wave in Waves)
            {
                if(wave.BossConfig != null)
                    enemyConfigs.Add(wave.BossConfig);
            }

            return enemyConfigs;
        }
    }
}