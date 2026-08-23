using System;
using System.Collections.Generic;
using System.Linq;
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
        private const string DamageTextPool = "DamageTextPool";
        private const string EnemyBullets = "EnemyBulletsPool";
        private const string LigtningBolt = "LigtningBoltPool";

        private readonly PoolServiceConfig _poolConfig;
        private readonly ObjectContainer _objectContainer;
        private readonly IObjectResolverProvider _resolverProvider;

        private List<EnemyConfig> _enemyConfigs = new List<EnemyConfig>();
        private List<LootInfo> _lootInfos = new List<LootInfo>();
        private CardsData _cardsData;
        private ParticleData _particleInfos;
        private LootData _lootData;
        private LevelConfig _levelConfig;
        private LightningBolt _ligtningBolt;
        private DamageText _damageText;

        private Dictionary<string, object> _objectPools = new Dictionary<string, object>();

        public PoolService (ObjectContainer objectContainer, IResourceLoader resourceLoader, IObjectResolverProvider resolverProvider)
        {
            _objectContainer = objectContainer;
            _poolConfig = resourceLoader.Load<PoolServiceConfig>();
            _cardsData = resourceLoader.Load<CardsData>();
            _damageText = resourceLoader.Load<DamageText>();
            _ligtningBolt = resourceLoader.Load<LightningBolt>();
            _particleInfos = resourceLoader.Load<ParticleData>();
            _lootData = resourceLoader.Load<LootData>();
            _resolverProvider = resolverProvider;
        }

        public void SetCurrentLevelConfig (List<EnemyConfig> currentEnemyConfig, LevelConfig levelConfig)
        {
            _enemyConfigs = currentEnemyConfig;
            _levelConfig = levelConfig;

           // Dictionary<string, LootInfo> lootInfos = new Dictionary<string, LootInfo>();
           //
           // foreach (EnemyConfig config in _enemyConfigs)
           // {
           //     foreach (LootInfo lootInfo in config.PossibleLoot)
           //     {
           //         if (lootInfo?.LootPrefab != null)
           //         {
           //             lootInfos.TryAdd(lootInfo.LootPrefab.name, lootInfo);
           //         }
           //     }
           // }
           //
           // _lootInfos = lootInfos.Values.ToList();
        }

        public T GetObject<T> (string nameObject)
            where T : Component
            => GetPool<T>(nameObject)?.Get();

        public T GetObject<T> (string nameObject, Vector3 position)
            where T : Component
            => GetPool<T>(nameObject)?.Get(position);

        public T GetObject<T> (string nameObject, Vector3 position, Quaternion rotation)
            where T : Component
            => GetPool<T>(nameObject)?.Get(position, rotation);

        public T GetObject<T> (string nameObject, Transform parent)
            where T : Component
            => GetPool<T>(nameObject)?.Get(parent);

        public T GetObject<T> (string nameObject, Vector3 position, Quaternion rotation, Transform parent)
            where T : Component
            => GetPool<T>(nameObject)?.Get(position, rotation, parent);

        public void ReleaseObject<T> (T obj)
            where T : Component
        {
            GetPool<T>(obj.name)?.Release(obj);
        }

        public void ClearAllPools ()
        {
            foreach (ObjectPoolBase obj in _objectPools.Values)
            {
                obj.Clear();
            }

            _objectPools.Clear();
        }

        public void InitializePools ()
        {
            CreatePoolContainer(EnemyPool, out Transform enemyPoolContainer);
            CreatePoolContainer(BallPool, out Transform ballPoolContainer);
            CreatePoolContainer(TilePool, out Transform tilePoolContainer);
            CreatePoolContainer(LootPool, out Transform lootPoolContainer);
            CreatePoolContainer(ParticlePool, out Transform particlePoolContainer);
            CreatePoolContainer(DamageTextPool, out Transform damageTextContainer);
            CreatePoolContainer(LigtningBolt, out Transform ligtningBoltContainer);
            CreatePoolContainer(EnemyBullets, out Transform enemyBulletsContainer);
            //CreatePoolContainer(CardPool, out Transform cardContainer);

            foreach (EnemyConfig config in _enemyConfigs)
                CreatePool(config.Prefab, enemyPoolContainer, config.name, _poolConfig.InitialEnemyPoolSize);

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

            CreatePool(_ligtningBolt, ligtningBoltContainer, LigtningBolt, _poolConfig.DefaultPoolSize);
            CreatePool(_damageText, damageTextContainer, DamageTextPool, _poolConfig.IinitialDamageTextPoolSize);
        }

        private void CreatePoolContainer (string name, out Transform container)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(_objectContainer.transform);
            container = go.transform;
        }

        private void CreatePool<T> (T prefab, Transform parent, string name, int initialSize)
            where T : Component
        {
            IObjectPool<T> pool = new ObjectPool<T>(initialSize, _resolverProvider);
            pool.InitializePool(prefab, parent, name);
            _objectPools[name] = pool;
        }

        private ObjectPool<T> GetPool<T> (string name)
            where T : Component
        {
            if (_objectPools.TryGetValue(name, out object poolObj) && poolObj is ObjectPool<T> pool)
                return pool;

            throw new ArgumentNullException(name);
        }
    }
}