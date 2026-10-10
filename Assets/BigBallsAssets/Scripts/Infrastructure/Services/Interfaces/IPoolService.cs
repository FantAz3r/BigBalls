using System.Collections.Generic;
using BigBalls.Configs;
using UnityEngine;

namespace BigBalls.Services
{
    public interface IPoolService
    {
        void SetCurrentLevelConfig (List<EnemyConfig> currentEnemyConfig, LevelConfig levelConfig);
        void InitializePools ();
        void Release<T> (T obj) where T : Component;
        void ClearAllPools ();
        T Get<T>(T prefab) where T : Component;
        T Get<T>(T prefab, Vector3 position) where T : Component;
        T Get<T>(T prefab, Vector3 position, Quaternion rotation) where T : Component;
        T Get<T>(T prefab, Transform parent) where T : Component;
        T Get<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent) where T : Component;
    }
}