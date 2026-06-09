using BigBalls.Configs;
using BigBalls.Infrastructure.DI;
using UnityEngine;

namespace BigBalls.Services
{
    public interface IPoolService
    {
        void SetCurrentLevelConfig(LevelConfig levelConfig);
        void InitializePools();
        T GetObject<T>(string name) where T : MonoBehaviour;
        void ReleaseObject<T>(T obj) where T : MonoBehaviour;
        void ClearAllPools();
    }
}