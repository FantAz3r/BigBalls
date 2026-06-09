using BigBalls.Factories;
using BigBalls.GameplayObjects;
using BigBalls.Providers;
using BigBalls.Services;
using UnityEngine;
using VContainer;
using VContainer.Unity;

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

            builder.Register<TileMover>(Lifetime.Scoped);
            builder.Register<IEntityRepository, EntityRepository>(Lifetime.Scoped);

            RegisterServices(builder);
            RegisterFactories(builder);
            RegisterProviders(builder);
        }

        private void RegisterFactories(IContainerBuilder builder)
        {
            builder.Register<IPlayerFactory, PlayerFactory>(Lifetime.Scoped);
            builder.Register<IBallEffectFactory, BallEffectFactory>(Lifetime.Scoped);
            builder.Register<IEnemyFactory, EnemyFactory>(Lifetime.Scoped);
            builder.Register<IBallFactory, BallFactory>(Lifetime.Scoped);
            builder.Register<TileFactory>(Lifetime.Scoped);
            builder.Register<EnemySpawner>(Lifetime.Scoped);
        }

        private void RegisterServices(IContainerBuilder builder)
        {
            builder.Register<IPoolService, PoolService>(Lifetime.Scoped);
            builder.Register<InputService>(Lifetime.Singleton)
                .AsImplementedInterfaces();
            builder.Register<IDamageService, DamageService>(Lifetime.Scoped);
            builder.Register<IRaycastService, RaycastService>(Lifetime.Transient);
            builder.Register<ILouseService, LouseService>(Lifetime.Scoped);
            builder.Register<IWinService, WinService>(Lifetime.Scoped);
        }

        private void RegisterProviders(IContainerBuilder builder)
        {
            builder.Register<IPlayerProvider, PlayerProvider>(Lifetime.Scoped);
            builder.RegisterComponent(_sceneContainer)
                .As<ISceneContainerProvider>();
        }
    }
}

