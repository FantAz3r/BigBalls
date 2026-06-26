using BigBalls.Factories;
using BigBalls.Services;
using UnityEngine;
using UnityEngine.Splines.ExtrusionShapes;
using VContainer;
using VContainer.Unity;

namespace BigBalls.Infrastructure.DI
{
    public class ProjectLifetimeScope : LifetimeScope
    {
        [SerializeField] private CoroutineRunner _coroutineRunner;
        [SerializeField] private UpdateService _updateService;
        private IGameStateMachine _gameStateMashine;

        protected override void Configure (IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<EntryPoint>(Lifetime.Singleton);
            builder.Register<IObjectResolverProvider, ObjectResolverProvider>(Lifetime.Singleton);
            builder.Register<ISceneLoader, SceneLoader>(Lifetime.Singleton);
            builder.Register<IGameStateMachine, GameStateMachine>(Lifetime.Singleton);

            BindStates(builder);
            BindServices(builder);
        }

        private void BindStates (IContainerBuilder builder)
        {
            builder.Register<LoadingLevelState>(Lifetime.Singleton);
            builder.Register<GameExitState>(Lifetime.Singleton);
        }

        private void BindServices (IContainerBuilder builder)
        {
            
            builder.Register<ITimeService, TimeService>(Lifetime.Singleton);
            builder.Register<ITranslateService, YGTranslateService>(Lifetime.Singleton);
            builder.Register<ILevelLoadingService, LevelLoadingService>(Lifetime.Singleton);
            builder.Register<IResourceLoader, ResourceLoader>(Lifetime.Singleton);
            builder.Register<IUIFactory, UIFactory>(Lifetime.Singleton);
            builder.Register<IWindowService, WindowService>(Lifetime.Singleton);
            builder.Register<IIdentifierService, IdentifierService>(Lifetime.Scoped);
            builder.Register<ISaveService, YGSaveService>(Lifetime.Singleton);

            builder.RegisterComponent(_updateService).As<IUpdateService>();
            builder.RegisterComponent(_coroutineRunner).As<ICoroutineRunner>();

            //................
            builder.Register<BallsRepository>(Lifetime.Singleton); // не уверен на счёт этого скоупа
            builder.Register<ArtefactsRepository>(Lifetime.Singleton); // не уверен на счёт этого скоупа

            builder.Register<BallTreeModel>(Lifetime.Singleton);
            builder.Register<GlobalWallet>(Lifetime.Singleton);
            builder.Register<IBallUnlockService, BallUnlockService>(Lifetime.Singleton);

        }
               
        protected override void OnDestroy ()
        {
            _gameStateMashine = Container.Resolve<IGameStateMachine>();
            _gameStateMashine.EnterIn<GameExitState>();
            base.OnDestroy();
        }
    }
}
