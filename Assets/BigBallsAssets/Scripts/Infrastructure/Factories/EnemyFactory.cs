using BigBalls.GameplayObjects;
using BigBalls.Infrastructure.DI;
using BigBalls.Services;
using BigBalls.StaticData;
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

        public EnemyFactory(
            IObjectResolverProvider resolverProvider,
            IIdentifierService identifierService,
            IUpdateService updateService,
            IRaycastService raycastService
            )
        {
            _resolverProvider = resolverProvider;
            _identifierService = identifierService;
            _updateService = updateService;
            _raycastService = raycastService;
        }

        public Enemy Create(EnemyConfig enemyConfig, Vector3 spawnPosition)
        {
            Enemy enemy = _resolverProvider.CurrentResolver.Instantiate(enemyConfig.Prefab, spawnPosition, Quaternion.identity);

            int playerID = _identifierService.ID;

            enemy.Construct(playerID);
            StatHolder statHolder = new StatHolder(playerID, enemyConfig);
            Mover mover = new Mover(statHolder[StatType.MoveSpeed], enemy.transform, _updateService, enemyConfig, _raycastService);
            EnemyMover enemyMover = new EnemyMover(mover, enemyConfig);

            return enemy;
        }
    }
}
