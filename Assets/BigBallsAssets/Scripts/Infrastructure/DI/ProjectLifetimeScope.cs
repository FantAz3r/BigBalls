using UnityEngine;
using VContainer;
using VContainer.Unity;
using BigBalls.Services;
using BigBalls.Factories;

namespace BigBalls.Infrastructure.DI
{
    public class ProjectLifetimeScope : LifetimeScope
    {
        [SerializeField] private CoroutineRunner _coroutineRunner;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<EntryPoint>(Lifetime.Singleton);

            builder.Register<ISceneLoader, SceneLoader>(Lifetime.Singleton);

            builder.Register<ITimeService, TimeService>(Lifetime.Singleton);
            builder.Register<IGameStateMachine, GameStateMachine>(Lifetime.Singleton);

            builder.RegisterComponent(_coroutineRunner).As<ICoroutineRunner>();

            builder.Register<ILevelLoadingService, LevelLoadingService>(Lifetime.Singleton);

            builder.Register<IUIFactory, UIFactory>(Lifetime.Singleton);
            builder.Register<IWindowService, WindowService>(Lifetime.Singleton);

        }
    }
}
