using System;
using System.Collections.Generic;
using BigBalls.Configs;
using BigBalls.GameplayObjects;
using BigBalls.Infrastructure.DI;
using BigBalls.StaticData;
using UnityEngine;

namespace BigBalls.Services
{
    public class PoolService : IPoolService
    {
        private readonly PoolServiceConfig _config;
        private readonly ObjectContainer _objectContainer;
        
        private EnemyData _enemyData;
        private BallsData _ballsData;

        private Dictionary<string, object> _objectPools = new Dictionary<string, object>();

        // private Dictionary<FactoryType, Dictionary<Type, ObjectPool<T>>> _objectPools;

        public PoolService(ObjectContainer  objectContainer, IResourceLoader resourceLoader, IObjectResolverProvider resolverProvider)
        {
            _objectContainer = objectContainer;
            _enemyData = resourceLoader.Load<EnemyData>();
            _config = resourceLoader.Load<PoolServiceConfig>();
            _ballsData = resourceLoader.Load<BallsData>();
            
            InitializePools(resolverProvider);
        }

        public T GetObject<T>(string nameObject) where T : MonoBehaviour
        {
            if (_objectPools.TryGetValue(nameObject, out object poolObj))
            {
                var pool = poolObj as ObjectPool<T>;
                
                if (pool != null)
                {
                    return pool.Get();
                }
            }

            return null;
        }

        public void ReleaseObject<T>(T obj) where T : MonoBehaviour
        {
            if (_objectPools.TryGetValue(obj.name, out object poolObj))
            {
                var pool = poolObj as ObjectPool<T>;
                
                if (pool != null)
                {
                    pool.Release(obj);
                }
            }
        }

        public void ClearAllPools()
        {
            foreach (ObjectPoolBase obj in _objectPools.Values)
            {
                obj.Clear();
            }
            
            _objectPools.Clear();
        }

        private void InitializePools(IObjectResolverProvider resolverProvider)
        {
            GameObject enemyPool = new GameObject("EnemyPool");
            GameObject bollPool = new GameObject("BollPool");
            
            enemyPool.transform.SetParent(_objectContainer.transform);
            bollPool.transform.SetParent(_objectContainer.transform);
            
            foreach (var config in _enemyData.Configs)
            {
                IObjectPool<Enemy> pool = new ObjectPool<Enemy>(_config.InitialEnemyPoolSize, resolverProvider);
                string nameParent = config.name;
                pool.InitializePool(config.Prefab, enemyPool.transform, nameParent);
                _objectPools[nameParent] = pool;
            }
        }
    }
}