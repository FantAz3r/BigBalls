using System.Collections.Generic;
using BigBalls.Configs;
using UnityEngine;

namespace BigBalls.Services
{
    public interface IPoolService
    {
        void SetCurrentLevelConfig (List<EnemyConfig> currentEnemyConfig, LevelConfig levelConfig);
        void InitializePools ();
        void ReleaseObject<T> (T obj) where T : MonoBehaviour;
        void ClearAllPools ();

        T GetObject<T> (string name)
            where T : MonoBehaviour;
        T GetObject<T> (string nameObject, Vector3 position)
            where T : MonoBehaviour;
        T GetObject<T> (string nameObject, Vector3 position, Quaternion rotation)
            where T : MonoBehaviour;
        T GetObject<T> (string nameObject, Transform parent)
            where T : MonoBehaviour;
        T GetObject<T> (string nameObject, Vector3 position, Quaternion rotation, Transform parent)
            where T : MonoBehaviour;
    }
}