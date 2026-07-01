using System.Collections.Generic;
using BigBalls.Configs;
using UnityEngine;

namespace BigBalls.Services
{
    public interface IPoolService
    {
        void SetCurrentLevelConfig(List<EnemyConfig> currentEnemyConfig, LevelConfig levelConfig);
        void InitializePools();
        T GetObject<T>(string name) where T : MonoBehaviour;
        void ReleaseObject<T>(T obj) where T : MonoBehaviour;
        void ClearAllPools();
    }
}