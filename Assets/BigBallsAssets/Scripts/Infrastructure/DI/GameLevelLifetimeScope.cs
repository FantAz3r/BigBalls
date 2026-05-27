using UnityEngine;
using VContainer;
using VContainer.Unity;
using BigBalls.Factories;
using BigBalls.GameplayObjects;
using BigBalls.Providers;
using BigBalls.Services;

namespace BigBalls.Infrastructure.DI
{
    public class GameLevelLifetimeScope : LifetimeScope
    {
        [SerializeField] private SceneContainerProvider _sceneContainer;
        [SerializeField] private ObjectContainer _objectContainer;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<LevelEntryPoint>(Lifetime.Scoped);
            builder.Register<CreateLevelState>(Lifetime.Scoped);

            builder.RegisterComponent(_objectContainer);
            builder.RegisterComponent(_sceneContainer)
                .As<ISceneContainerProvider>();

            builder.Register<InputService>(Lifetime.Singleton)
                .AsImplementedInterfaces();

            builder.Register<IPoolService, PoolService>(Lifetime.Scoped);
            builder.Register<TileMover>(Lifetime.Scoped);

            builder.Register<IEntityRepository, EntityRepository>(Lifetime.Scoped);

            RegisterFactories(builder);
        }

        private void RegisterFactories(IContainerBuilder builder)
        {
            builder.Register<IPlayerFactory, PlayerFactory>(Lifetime.Scoped);
            builder.Register<TileFactory>(Lifetime.Scoped);
            builder.Register<IBallBehaivorFactory, BallBehaivorFactory>(Lifetime.Scoped);
            builder.Register<IEnemyFactory, EnemyFactory>(Lifetime.Scoped);

        }
    }
}

