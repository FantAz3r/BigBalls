using BigBalls.Infrastructure;
using BigBalls.Services;
using UnityEngine;
using VContainer;

namespace BigBalls.UI
{
    public class LevelButton : ButtonClickHandler
    {
        [SerializeField] private WindowBase _closeWindow;
        [field: SerializeField] public LevelID LevelID { get; private set; }

        private ILevelLoadingService _loadingService;
        private IWindowService _windowService;
        private CreateLevelState _createLevelState;

        [Inject]
        public void Construct (ILevelLoadingService levelLoadingService, IWindowService windowService, CreateLevelState createLevelState)
        {
            _loadingService = levelLoadingService;
            _windowService = windowService;
            _createLevelState = createLevelState;
        }

        public void Set (LevelID level) => LevelID = level;

        protected override void OnClick ()
        {
            if (LevelID == LevelID.None)
            {
                _loadingService.Load(_loadingService.CurrentLevel);
            }
            else if (_createLevelState.CurrentLevel == LevelID)
            {

                if (_createLevelState.IsGameStarted)
                {
                    _loadingService.Load(LevelID);
                }
                else
                {
                    _closeWindow?.Close();
                    _windowService.Open<MainMenu>();
                }
            }
            else
            {
                _loadingService.Load(LevelID);
            }
        }
    }
}
