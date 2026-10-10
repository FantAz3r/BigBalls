using BigBalls.Infrastructure;
using BigBalls.Infrastructure.DI;
using UnityEngine;

namespace BigBalls.Services
{
    public class LevelLoadingService : ILevelLoadingService
    {
        private readonly IGameStateMachine _stateMachine;

        public LevelLoadingService(IGameStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }
        public LevelID CurrentLevel { get; private set; } = LevelID.Level1;

        public void Load(LevelID level)
        {
            CurrentLevel = level;
            _stateMachine.EnterIn<LoadingLevelState, LevelID>(level);
        }
    }
}
