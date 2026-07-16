using BigBalls.Configs;
using BigBalls.GameplayObjects;
using BigBalls.Infrastructure.DI;
using BigBalls.Services;
using System.Collections.Generic;
using UnityEngine;

namespace BigBalls.Factories
{
    public class EnemyFactory : IEnemyFactory
    {
        private readonly IObjectResolverProvider _resolverProvider;
        private readonly IIdentifierService _identifierService;
        private readonly IUpdateService _updateService;
        private readonly IRaycastService _raycastService;
        private readonly IPlayerProvider _playerProvider;
        private readonly IDamageService _damageService;
        private readonly IEntityRepository _entityRepository;
        private readonly IPoolService _poolService;
        private readonly IDropService _dropService;

        public EnemyFactory(
            IObjectResolverProvider resolverProvider,
            IIdentifierService identifierService,
            IUpdateService updateService,
            IRaycastService raycastService,
            IPlayerProvider playerProvider,
            IDamageService damageService,
            IEntityRepository entityRepository,
            IPoolService poolService,
            IDropService dropService
            )
        {
            _resolverProvider = resolverProvider;
            _identifierService = identifierService;
            _updateService = updateService;
            _raycastService = raycastService;
            _playerProvider = playerProvider;
            _damageService = damageService;
            _entityRepository = entityRepository;
            _poolService = poolService;
            _dropService = dropService;
        }

        public Enemy Create(EnemyConfig enemyConfig, Vector3 spawnPosition)
        {
            Enemy enemy = _poolService.GetObject<Enemy>(enemyConfig.name);
            enemy.transform.position = spawnPosition;
            enemy.transform.rotation = Quaternion.identity;
            int enemyID = _identifierService.ID;

            StatHolder statHolder = new StatHolder(enemy, enemyConfig.Type, enemyConfig);
            statHolder.InitStats();

            List<ISubscribable> subscribables = CreateComponents(statHolder, enemy, enemyConfig);

            DeathHandler<Enemy> deathHandler = new DeathHandler<Enemy>(statHolder[StatType.Health], subscribables, enemy);
            deathHandler.Subscribe();
            enemy.EventHandler.Died += OnDied;

            enemy.Construct(enemyID, deathHandler);
            _entityRepository.Add(enemy, statHolder);

            enemy.HitFlash.Init(statHolder[StatType.Health]);

            return enemy;
        }

        private void OnDied(IEntity entity)
        {
            if (entity is not Enemy enemy)
                return;

            if (enemy.DeathHandler.IsBallAttacked)
                _dropService.DropLoot(enemy.transform.position, enemy);

            enemy.DeathHandler.Unsubscribe();
            enemy.EventHandler.Died -= OnDied;
            enemy.EventHandler.Suisided -= OnDied;

            _poolService.ReleaseObject(enemy);
            _entityRepository.Remove(enemy);
        }

        private List<ISubscribable> CreateComponents(StatHolder statHolder, Enemy enemy, EnemyConfig enemyConfig)
        {
            Mover mover = new Mover(statHolder[StatType.MoveSpeed], enemy.transform, _updateService, _raycastService, enemyConfig);
            Init(mover, enemyConfig);

            EnemyAttacker enemyAttacker = new EnemyAttacker(_playerProvider, statHolder[StatType.Damage], enemy, enemy.EntityTrigger, _damageService);
            enemy.EventHandler.Suisided += OnDied;

            List<ISubscribable> subscribables = new List<ISubscribable>()
            {
                mover,
                enemyAttacker
            };

            return subscribables;
        }

        private void Init(Mover mover, EnemyConfig enemyConfig)
        {
            mover.SetDirection(new Vector2(0, -1));

            List<Vector2Int> frontRowBlocks = enemyConfig.GetBlocksWithoutFrontNeighbor();

            Vector3[] raycastPoints = new Vector3[frontRowBlocks.Count];
            Vector3[] raycastDirections = new Vector3[frontRowBlocks.Count];

            for (int i = 0; i < frontRowBlocks.Count; i++)
            {
                Vector2Int blockPos = frontRowBlocks[i];
                raycastPoints[i] = new Vector3(blockPos.x, 0.25f, blockPos.y);
                raycastDirections[i] = Vector3.back;
            }

            mover.SetReycastInfo(raycastDirections, raycastPoints);
        }
    }
}
