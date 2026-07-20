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

        protected override void OnClick()
        {
             _loadingService.Load(LevelID);
        }
    }
}
