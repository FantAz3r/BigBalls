using BigBalls.Services;
using BigBalls.UI;

namespace BigBalls.Infrastructure
{
    public class MainMenuState : IState
    {
        private readonly IWindowService _windowService;
        private readonly BallsRepository _ballsRepository;
        private readonly ISaveService _saveService;

        public MainMenuState(
            IWindowService windowService,
            BallsRepository ballsRepository,
            ISaveService saveService)
        {
            _windowService = windowService;
            _ballsRepository = ballsRepository;
            _saveService = saveService;
        }

        public void Enter()
        {
            _windowService.CreateUIRoot();
            _windowService.Open<MainMenu>();
        }

        public void Exit()
        {
            _saveService.Save();
        }
    }
}