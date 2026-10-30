using System;
using BigBalls.Infrastructure.DI;
using VContainer;
using VContainer.Unity;

namespace BigBalls.Infrastructure
{
    public class EntryPoint : IStartable
    {
        private readonly ISaveService _saveService;
        private IGameStateMachine _gameStateMachine;

        public EntryPoint (IGameStateMachine gameStateMachine, IObjectResolverProvider objectResolverProvider, IObjectResolver objectResolver, ISaveService saveService)
        {
            objectResolverProvider.UpdateResolver(objectResolver);
            _gameStateMachine = gameStateMachine ?? throw new ArgumentNullException(nameof(gameStateMachine));
            _saveService = saveService;
        }

        public void Start ()
        {
            _saveService.Load();
            int level = _saveService.GameProgress.Level;

            if (level == 0)
            {
                _gameStateMachine.EnterIn<LoadingLevelState, LevelID>(LevelID.Level1);
            }
            else
            {
                _gameStateMachine.EnterIn<LoadingLevelState, LevelID>((LevelID) level);
            }
        }
    }
}
