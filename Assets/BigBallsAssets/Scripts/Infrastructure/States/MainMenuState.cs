using BigBalls.Services;
using BigBalls.UI;

namespace BigBalls.Infrastructure
{
    public class MainMenuState : IState
    {
        private readonly IWindowService _windowService;
        private readonly BallRepository _ballsRepository;
        private readonly ISaveService _saveService;
        private readonly ArtefactsRepository _artefactsRepository;
        private readonly IUIFactory _uIFactory;

        public MainMenuState(
            IWindowService windowService,
            BallRepository ballsRepository,
            ISaveService saveService,
            ArtefactsRepository artefactsRepository,
            IUIFactory uIFactory)
        {
            _windowService = windowService;
            _ballsRepository = ballsRepository;
            _saveService = saveService;
            _artefactsRepository = artefactsRepository;
            _uIFactory = uIFactory;
        }

        public void Enter()
        {
        }

        public void Exit()
        {
            
        }
    }
}