using BigBalls.Services;
using BigBalls.UI;

namespace BigBalls.Infrastructure
{
    public class MainMenuState : IState
    {
        private readonly IWindowService _windowService;

        public MainMenuState( IWindowService windowService)
        {
            _windowService = windowService;
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