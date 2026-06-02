using BigBalls.GameplayObjects;
using BigBalls.Infrastructure.DI;
using BigBalls.Services;
using BigBalls.StaticData;
using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;

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

        public EnemyFactory(
            IObjectResolverProvider resolverProvider,
            IIdentifierService identifierService,
            IUpdateService updateService,
            IRaycastService raycastService,
            IPlayerProvider playerProvider,
            IDamageService damageService
            )
        {
            _resolverProvider = resolverProvider;
            _identifierService = identifierService;
            _updateService = updateService;
            _raycastService = raycastService;
            _playerProvider = playerProvider;
            _damageService = damageService;
        }

        public Enemy Create(EnemyConfig enemyConfig, Vector3 spawnPosition)
        {
            Enemy enemy = _resolverProvider.CurrentResolver.Instantiate(enemyConfig.Prefab, spawnPosition, Quaternion.identity);
            int playerID = _identifierService.ID;

            StatHolder statHolder = new StatHolder(playerID, enemyConfig);
            Mover mover = new Mover(statHolder[StatType.MoveSpeed], enemy.transform, _updateService, enemyConfig, _raycastService);
            Init(mover, enemyConfig);

            EnemyAttacker enemyAttacker = new EnemyAttacker(_playerProvider, statHolder[StatType.Damage], enemy.transform, enemy.EntityTrigger, _damageService);

            List<ISubscribable> subscribables = new List<ISubscribable>()
            {
                mover,
                enemyAttacker
            };

            DeathHandler deathHandler = new DeathHandler(statHolder[StatType.Health], subscribables);
            enemy.Construct(playerID, deathHandler);

            return enemy;
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
