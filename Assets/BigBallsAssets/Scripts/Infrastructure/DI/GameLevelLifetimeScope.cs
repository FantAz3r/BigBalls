using BigBalls.Factories;
using BigBalls.GameplayObjects;
using BigBalls.Providers;
using BigBalls.Services;
using UnityEngine;
using VContainer;
using VContainer.Unity;
//using YG;

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

            builder.Register<IPlayerFactory, PlayerFactory>(Lifetime.Scoped);
            builder.Register<IPlayerProvider, PlayerProvider>(Lifetime.Scoped);
            builder.RegisterComponent(_sceneContainer).As<ISceneContainerProvider>();

            builder.RegisterComponent(_objectContainer);

            builder.Register<InputService>(Lifetime.Singleton)
                .AsImplementedInterfaces();

            builder.Register<IPoolService, PoolService>(Lifetime.Scoped);
            builder.Register<TileFactory>(Lifetime.Scoped);
            builder.Register<TileMover>(Lifetime.Scoped);

        }
    }
}

