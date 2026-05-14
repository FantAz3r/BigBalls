using UnityEngine;
using VContainer;
using VContainer.Unity;

public class ProjectLifetimeScope : LifetimeScope
{
    [SerializeField] private CoroutineRunner _coroutineRunner;

    protected override void Configure(IContainerBuilder builder)
    {

        builder.RegisterEntryPoint<EntryPoint>(Lifetime.Singleton);

        builder.Register<ISceneLoader, SceneLoader>(Lifetime.Singleton);

        builder.Register<IGameStateMachine, GameStateMachine>(Lifetime.Singleton);

        builder.RegisterComponent(_coroutineRunner)
               .As<ICoroutineRunner>();

        builder.Register<ILevelLoadingService, LevelLoadingService>(Lifetime.Singleton);
    }
}
