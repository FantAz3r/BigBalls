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
        private readonly IObjectResolverProvider _resolverProvider;

        private EnemyData _enemyData;
        private BallsData _ballsData;
        private LevelConfig _levelConfig;

        private Dictionary<string, object> _objectPools = new Dictionary<string, object>();

        public PoolService(ObjectContainer objectContainer, IResourceLoader resourceLoader, IObjectResolverProvider resolverProvider)
        {
            _objectContainer = objectContainer;
            _enemyData = resourceLoader.Load<EnemyData>();
            _config = resourceLoader.Load<PoolServiceConfig>();
            _ballsData = resourceLoader.Load<BallsData>();
            _resolverProvider = resolverProvider;
        }

        public void SetCurrentLevelConfig(LevelConfig levelConfig)
        {
            _levelConfig = levelConfig;
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

        public void InitializePools()
        {
            CreatePoolContainer("EnemyPool", out Transform enemyPoolContainer);
            CreatePoolContainer("BallPool", out Transform ballPoolContainer);
            CreatePoolContainer("TilePool", out Transform tilePoolContainer);

            foreach (EnemyConfig config in _enemyData.Configs)
            {
                IObjectPool<Enemy> pool = new ObjectPool<Enemy>(_config.InitialEnemyPoolSize, _resolverProvider);
                string nameParent = config.name;
                pool.InitializePool(config.Prefab, enemyPoolContainer.transform, nameParent);
                _objectPools[nameParent] = pool;
            }

            foreach (BallConfig config in _ballsData.BallConfigsList)
            {
                IObjectPool<Ball> pool = new ObjectPool<Ball>(_config.InitialBallPoolSize, _resolverProvider);
                string nameParent = config.name;
                pool.InitializePool(config.Prefab, ballPoolContainer.transform, nameParent);
                _objectPools[nameParent] = pool;
            }

            foreach (Tile tile in _levelConfig.TilePrefabs)
            {
                IObjectPool<Tile> pool = new ObjectPool<Tile>(_config.InitialTilePoolSize, _resolverProvider);
                string nameParent = tile.name;
                pool.InitializePool(tile, tilePoolContainer.transform, nameParent);
                _objectPools[nameParent] = pool;
            }
        }

        private void CreatePoolContainer(string name, out Transform container)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(_objectContainer.transform);
            container = go.transform;
        }
    }
}