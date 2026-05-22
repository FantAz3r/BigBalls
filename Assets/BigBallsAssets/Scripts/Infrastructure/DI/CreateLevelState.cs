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

        public CreateLevelState(
            IPlayerFactory playerFactory,
            IWindowService windowService,
            IUpdateService updateService,
            IUIFactory uIFactory
            )
        {
            _playerFactory = playerFactory;
            _windowService = windowService;
            _updateService = updateService;
            _uIFactory = uIFactory;
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
        }
    }
}