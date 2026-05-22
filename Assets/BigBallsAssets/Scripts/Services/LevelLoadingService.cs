using BigBalls.Infrastructure;
using BigBalls.Infrastructure.DI;

namespace BigBalls.Services
{
    public class LevelLoadingService : ILevelLoadingService
    {
        private readonly IGameStateMachine _stateMachine;

        public LevelLoadingService(IGameStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void Load(LevelID level)
        {
            if(level == LevelID.MainMenu)
            {
                _stateMachine.EnterIn<MainMenuState>();
            }
            else
            {
                _stateMachine.EnterIn<LoadingLevelState, LevelID>(level);
            }
        }
    }
}
