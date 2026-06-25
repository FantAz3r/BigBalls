using BigBalls.Services;
using BigBalls.UI;

namespace BigBalls.Infrastructure
{
    public class MainMenuState : IState
    {
        private readonly IWindowService _windowService;
        private readonly BallsRepository _ballsRepository;

        public MainMenuState(
            IWindowService windowService,
            BallsRepository ballsRepository)
        {
            _windowService = windowService;
            _ballsRepository = ballsRepository;
        }

        public void Enter()
        {
            _windowService.CreateUIRoot();
            _windowService.Open<MainMenu>();
        }

        public void Exit()
        {
        }
    }
}