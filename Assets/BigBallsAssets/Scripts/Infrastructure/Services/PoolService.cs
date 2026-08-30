using System;
using System.Collections.Generic;
using BigBalls.Configs;
using BigBalls.GameplayObjects;
using BigBalls.Infrastructure.DI;
using UnityEngine;

namespace BigBalls.Services
{
    public class PoolService : IPoolService
    {
        private const string EnemyPool = "EnemyPool";
        private const string BallPool = "BallPool";
        private const string TilePool = "TilePool";
        private const string LootPool = "LootPool";
        private const string ParticlePool = "ParticlePool";
        private const string EnemyBullets = "EnemyBulletsPool";
        private const string MiscPool = "MiscPool";

        private readonly PoolServiceConfig _poolConfig;
        private readonly ObjectContainer _objectContainer;
        private readonly IObjectResolverProvider _resolverProvider;

        private List<EnemyConfig> _enemyConfigs = new List<EnemyConfig>();
        private List<LootInfo> _lootInfos = new List<LootInfo>();
        private CardsData _cardsData;
        private ParticleData _particleInfos;
        private LootData _lootData;
        private LevelConfig _levelConfig;
        private LightningBolt _lightningBolt;
        private DamageText _damageText;

        private Dictionary<string, object> _objectPools = new Dictionary<string, object>();
        private Transform _miscContainer;
        private HashSet<string> _initializedPools = new HashSet<string>();

        public PoolService(ObjectContainer objectContainer, IResourceLoader resourceLoader, IObjectResolverProvider resolverProvider)
        {
            _objectContainer = objectContainer;
            _poolConfig = resourceLoader.Load<PoolServiceConfig>();
            _cardsData = resourceLoader.Load<CardsData>();
            _damageText = resourceLoader.Load<DamageText>();
            _lightningBolt = resourceLoader.Load<LightningBolt>();
            _particleInfos = resourceLoader.Load<ParticleData>();
            _lootData = resourceLoader.Load<LootData>();
            _resolverProvider = resolverProvider;
        }

        public void SetCurrentLevelConfig(List<EnemyConfig> currentEnemyConfig, LevelConfig levelConfig)
        {
            _enemyConfigs = currentEnemyConfig;
            _levelConfig = levelConfig;
        }

        public T GetObject<T>(T prefab) where T : Component
        {
            if (prefab == null)
                throw new ArgumentNullException(nameof(prefab));
            return GetOrCreatePool(prefab)?.Get();
        }

        public T GetObject<T>(T prefab, Vector3 position) where T : Component
        {
            if (prefab == null)
                throw new ArgumentNullException(nameof(prefab));
            return GetOrCreatePool(prefab)?.Get(position);
        }

        public T GetObject<T>(T prefab, Vector3 position, Quaternion rotation) where T : Component
        {
            if (prefab == null) throw new ArgumentNullException(nameof(prefab));
            return GetOrCreatePool(prefab)?.Get(position, rotation);
        }

        public T GetObject<T>(T prefab, Transform parent) where T : Component
        {
            if (prefab == null) throw new ArgumentNullException(nameof(prefab));
            return GetOrCreatePool(prefab)?.Get(parent);
        }

        public T GetObject<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent) where T : Component
        {
            if (prefab == null)
                throw new ArgumentNullException(nameof(prefab));
            return GetOrCreatePool(prefab)?.Get(position, rotation, parent);
        }

        public void ReleaseObject<T>(T obj) where T : Component
        {
            if (obj == null) return;

            string poolName = obj.name.Replace("(Clone)", "").Trim();

            if (_objectPools.TryGetValue(poolName, out object poolObj) && poolObj is ObjectPool<T> pool)
            {
                pool.Release(obj);
            }
            else
            {
                TryReleaseToMiscPool(obj);
            }
        }

        public void ClearAllPools()
        {
            foreach (ObjectPoolBase obj in _objectPools.Values)
            {
                obj.Clear();
            }
            _objectPools.Clear();
            _initializedPools.Clear();
        }

        public void InitializePools()
        {
            CreatePoolContainer(EnemyPool, out Transform enemyPoolContainer);
            CreatePoolContainer(BallPool, out Transform ballPoolContainer);
            CreatePoolContainer(TilePool, out Transform tilePoolContainer);
            CreatePoolContainer(LootPool, out Transform lootPoolContainer);
            CreatePoolContainer(ParticlePool, out Transform particlePoolContainer);
            CreatePoolContainer(EnemyBullets, out Transform enemyBulletsContainer);
            CreatePoolContainer(MiscPool, out _miscContainer);

            foreach (EnemyConfig config in _enemyConfigs)
                CreatePool(config.Prefab, enemyPoolContainer, config.Prefab.name, _poolConfig.InitialEnemyPoolSize);

            foreach (BallConfig config in _cardsData.Balls.Values)
                CreatePool(config.Prefab, ballPoolContainer, config.Prefab.name, _poolConfig.InitialBallPoolSize);

            foreach (BallConfig config in _cardsData.EnmyProjectiles.Values)
                CreatePool(config.Prefab, enemyBulletsContainer, config.Prefab.name, _poolConfig.InitialBallPoolSize);

            foreach (Tile tile in _levelConfig.TilePrefabs)
                CreatePool(tile, tilePoolContainer, tile.name, _poolConfig.InitialTilePoolSize);

            foreach (Loot loot in _lootData.GetAllLoot())
                CreatePool(loot, lootPoolContainer, loot.name, _poolConfig.IinitialLootPoolSize);

            foreach (ParticleObject particle in _particleInfos.Particles)
                CreatePool(particle, particlePoolContainer, particle.gameObject.name, _poolConfig.DefaultPoolSize);
        }

        private void CreatePoolContainer(string name, out Transform container)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(_objectContainer.transform);
            container = go.transform;
        }

        private void CreatePool<T>(T prefab, Transform parent, string poolName, int initialSize) where T : Component
        {
            if (string.IsNullOrEmpty(poolName))
                poolName = prefab.name;

            if (_objectPools.ContainsKey(poolName))
                return;

            IObjectPool<T> pool = new ObjectPool<T>(initialSize, _resolverProvider);
            pool.InitializePool(prefab, parent, poolName);
            _objectPools[poolName] = pool;
            _initializedPools.Add(poolName);
        }

        private ObjectPool<T> GetOrCreatePool<T>(T prefab) where T : Component
        {
            string poolName = prefab.name;

            if (_objectPools.TryGetValue(poolName, out object poolObj) && poolObj is ObjectPool<T> pool)
                return pool;

            int defaultSize = _poolConfig?.DefaultPoolSize ?? 5;
            CreatePool(prefab, _miscContainer, poolName, defaultSize);

            return GetPool<T>(poolName);
        }

        private ObjectPool<T> GetPool<T>(string name) where T : Component
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            string cleanName = name.Replace("(Clone)", "").Trim();

            if (_objectPools.TryGetValue(cleanName, out object poolObj) && poolObj is ObjectPool<T> pool)
                return pool;

            throw new ArgumentNullException($"Pool '{cleanName}' not found");
        }

        private void TryReleaseToMiscPool<T>(T obj) where T : Component
        {
            string typeName = typeof(T).Name;

            if (_objectPools.TryGetValue(typeName, out object poolObj) && poolObj is ObjectPool<T> pool)
            {
                pool.Release(obj);
            }
            else
            {
                UnityEngine.Object.Destroy(obj.gameObject);
            }
        }
    }
}