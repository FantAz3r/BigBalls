using BigBalls.Factories;
using BigBalls.Services;
using BigBalls.UI;

namespace BigBalls.Infrastructure
{
    public class CreateLevelState : IState
    {
        private readonly IPlayerFactory _playerFactory;
        private readonly IWindowService _windowService;
        private readonly IUpdateService _updateService;
        private readonly IUIFactory _uIFactory;
        private readonly ITimeService _timeService;

        public CreateLevelState(
            IPlayerFactory playerFactory,
            IWindowService windowService,
            IUpdateService updateService,
            IUIFactory uIFactory,
            ITimeService timeService
            )
        {
            _playerFactory = playerFactory;
            _windowService = windowService;
            _updateService = updateService;
            _uIFactory = uIFactory;
            _timeService = timeService;
        }

        public void Enter()
        {
            _windowService.CreateUIRoot();
            _playerFactory.Create();
            _windowService.Open<HUD>();
        }

        public void Exit()
        {
            _uIFactory.ClearCache();
            _updateService.Clear();
            _timeService.ResumeGame();

        }
    }
}