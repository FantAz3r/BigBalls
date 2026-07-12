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

        public MainMenuState(
            IWindowService windowService,
            BallRepository ballsRepository,
            ISaveService saveService,
            ArtefactsRepository artefactsRepository)
        {
            _windowService = windowService;
            _ballsRepository = ballsRepository;
            _saveService = saveService;
            _artefactsRepository = artefactsRepository;
        }

        public void Enter()
        {
            _windowService.CreateUIRoot();
            _windowService.Open<Background>();
            _windowService.Open<MainMenu>();
        }

        public void Exit()
        {
            _artefactsRepository.Save();
            _ballsRepository.Save();
            _saveService.Save();
        }
    }
}