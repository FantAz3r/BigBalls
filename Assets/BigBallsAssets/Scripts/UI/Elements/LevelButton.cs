using UnityEngine;
using VContainer;
using BigBalls.Services;
using BigBalls.Infrastructure;

namespace BigBalls.UI
{
    public class LevelButton : ButtonClickHandler
    {
        [field: SerializeField] public LevelID LevelID { get; private set; }

        private ILevelLoadingService _loadingService;

        [Inject]
        public void Construct(ILevelLoadingService levelLoadingService)
        {
            _loadingService = levelLoadingService;
        }

        public void Set (LevelID level) => LevelID = level;

        protected override void OnClick()
        {
            if(LevelID == LevelID.None)
            {
                _loadingService.Load(_loadingService.CurrentLevel);
            }
            else
            {
                _loadingService.Load(LevelID);
            }
        }
    }
}
