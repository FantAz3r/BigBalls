using BigBalls.Factories;
using BigBalls.GameplayObjects;
using BigBalls.Infrastructure.DI;
using BigBalls.Services;
using BigBalls.StaticData;
using VContainer.Unity;

namespace BigBalls.Factories
{
    public class EnemyFactory : IEnemyFactory
    {
        private readonly IObjectResolverProvider _resolverProvider;
        private readonly IIdentifierService _identifierService;
        private readonly IUpdateService _updateService;

        public EnemyFactory(
            IObjectResolverProvider resolverProvider,
            IIdentifierService identifierService,
            IUpdateService updateService
            )
        {
            _resolverProvider = resolverProvider;
            _identifierService = identifierService;
            _updateService = updateService;
        }

        public Enemy Create(EnemyConfig enemyConfig)
        {
            Enemy enemy = _resolverProvider.CurrentResolver.Instantiate(enemyConfig.Prefab);

            int playerID = _identifierService.ID;
            enemy.Construct(playerID);
            StatHolder statHolder = new StatHolder(playerID, enemyConfig);
            Mover mover = new Mover(statHolder[StatType.MoveSpeed], enemy.transform, _updateService, enemyConfig);


            return enemy;
        }
    }
}
