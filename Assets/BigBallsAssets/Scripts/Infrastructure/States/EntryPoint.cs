using BigBalls.Infrastructure.DI;
using System;
using VContainer;
using VContainer.Unity;

namespace BigBalls.Infrastructure
{
    public class EntryPoint : IStartable
    {
        private IGameStateMachine _gameStateMachine;

        public EntryPoint(IGameStateMachine gameStateMachine, IObjectResolverProvider objectResolverProvider, IObjectResolver objectResolver)
        {
            objectResolverProvider.UpdateResolver(objectResolver);
            _gameStateMachine = gameStateMachine ?? throw new ArgumentNullException(nameof(gameStateMachine));
        }

        public void Start()
        {
            _gameStateMachine.EnterIn<LoadingLevelState, LevelID>(LevelID.MainMenu);
        }
    }
}
