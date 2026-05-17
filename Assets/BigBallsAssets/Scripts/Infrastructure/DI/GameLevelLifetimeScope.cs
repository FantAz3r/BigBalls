using BigBalls.Factories;
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

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<LevelEntryPoint>(Lifetime.Scoped);

            builder.Register<IPlayerFactory, PlayerFactory>(Lifetime.Scoped);
            builder.Register<IPlayerProvider, PlayerProvider>(Lifetime.Scoped);
            builder.RegisterComponent(_sceneContainer).As<ISceneContainerProvider>();
            builder.Register<InputService>(Lifetime.Scoped)
                .AsImplementedInterfaces();

        }
    }
}

