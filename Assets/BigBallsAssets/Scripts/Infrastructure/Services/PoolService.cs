using System.Collections.Generic;
using System.Linq;
using BigBalls.Configs;
using BigBalls.GameplayObjects;
using BigBalls.Infrastructure.DI;
using BigBalls.StaticData;
using UnityEngine;

namespace BigBalls.Services
{
    public class PoolService : IPoolService
    {
        private const string EnemyPool = "EnemyPool";
        private const string BallPool = "BallPool";
        private const string TilePool = "TilePool";
        private const string LootPool = "LootPoo";
        
        private readonly PoolServiceConfig _poolConfig;
        private readonly ObjectContainer _objectContainer;
        private readonly IObjectResolverProvider _resolverProvider;

        private List<EnemyConfig> _enemyConfigs = new List<EnemyConfig>();
        private List<LootInfo> _lootInfos = new List<LootInfo>();
        private BallsData _ballsData;
        private LevelConfig _levelConfig;

        private Dictionary<string, object> _objectPools = new Dictionary<string, object>();

        public PoolService(ObjectContainer objectContainer, IResourceLoader resourceLoader, IObjectResolverProvider resolverProvider)
        {
            _objectContainer = objectContainer;
            _poolConfig = resourceLoader.Load<PoolServiceConfig>();
            _ballsData = resourceLoader.Load<BallsData>();
            _resolverProvider = resolverProvider;
        }

        public void SetCurrentLevelConfig(LevelConfig levelConfig)
        {
            _levelConfig = levelConfig;
            _enemyConfigs = _levelConfig.GetEnemiesForPool();

            Dictionary<string, LootInfo> lootInfos = new Dictionary<string, LootInfo>();
            
            foreach (EnemyConfig config in _enemyConfigs)
            {
                foreach (LootInfo lootInfo in config.PossibleLoot)
                {
                    if (lootInfo?.LootPrefab != null)  
                    {
                        lootInfos.TryAdd(lootInfo.LootPrefab.name, lootInfo);
                    }
                }
            }

            _lootInfos = lootInfos.Values.ToList();
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
            CreatePoolContainer(EnemyPool, out Transform enemyPoolContainer);
            CreatePoolContainer(BallPool, out Transform ballPoolContainer);
            CreatePoolContainer(TilePool, out Transform tilePoolContainer);
            CreatePoolContainer(LootPool, out Transform lootPoolContainer);

            foreach (EnemyConfig config in _enemyConfigs)
            {
                IObjectPool<Enemy> pool = new ObjectPool<Enemy>(_poolConfig.InitialEnemyPoolSize, _resolverProvider);
                string nameParent = config.name;
                pool.InitializePool(config.Prefab, enemyPoolContainer.transform, nameParent);
                _objectPools[nameParent] = pool;
            }

            foreach (BallConfig config in _ballsData.BallConfigsList)
            {
                IObjectPool<Ball> pool = new ObjectPool<Ball>(_poolConfig.InitialBallPoolSize, _resolverProvider);
                string nameParent = config.name;
                pool.InitializePool(config.Prefab, ballPoolContainer.transform, nameParent);
                _objectPools[nameParent] = pool;
            }

            foreach (Tile tile in _levelConfig.TilePrefabs)
            {
                IObjectPool<Tile> pool = new ObjectPool<Tile>(_poolConfig.InitialTilePoolSize, _resolverProvider);
                string nameParent = tile.name;
                pool.InitializePool(tile, tilePoolContainer.transform, nameParent);
                _objectPools[nameParent] = pool;
            }

            foreach (LootInfo lootInfo in _lootInfos)
            {
                IObjectPool<Loot> pool = new ObjectPool<Loot>(_poolConfig.IinitialLootPoolSize, _resolverProvider);
                string nameParent = lootInfo.LootPrefab.name;
                pool.InitializePool(lootInfo.LootPrefab, lootPoolContainer.transform, nameParent);
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