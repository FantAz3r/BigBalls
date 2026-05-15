using UnityEngine;
using BigBalls.Infrastructure;

namespace BigBalls.Services
{
    public class LevelLoadingService : ILevelLoadingService
    {
        private readonly IGameStateMachine _stateMachine;

        public LevelLoadingService(IGameStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
            Debug.Log("LevelLoadingService");
        }

        public void Load(LevelID level) => _stateMachine.EnterIn<LoadingLevelState, LevelID>(level);
    }
}
