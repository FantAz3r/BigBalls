using BigBalls.Infrastructure;
using BigBalls.Infrastructure.DI;
using UnityEngine;

namespace BigBalls.Services
{
    public class LevelLoadingService : ILevelLoadingService
    {
        private readonly IGameStateMachine _stateMachine;
        private LevelID _currentLevel;
        public LevelLoadingService(IGameStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void Load(LevelID level)
        {
            Debug.Log(level);
            _currentLevel = level;
            _stateMachine.EnterIn<LoadingLevelState, LevelID>(level);
        }

        public void Reload()
        {
            _stateMachine.EnterIn<LoadingLevelState, LevelID>(_currentLevel);
        }
    }
}
