using UnityEngine;
using VContainer;
using BigBalls.Services;
using BigBalls.Infrastructure;

namespace BigBalls.UI
{
    public class LevelButton : ButtonClickHandler
    {
        [SerializeField] private LevelID _levelID;

        private ILevelLoadingService _loadingService;

        [Inject]
        public void Construct(ILevelLoadingService levelLoadingService)
        {
            _loadingService = levelLoadingService;
            Debug.Log(_loadingService);
        }

        protected override void OnClick()
        {
            Debug.Log("123");
             _loadingService.Load(_levelID);

        }
    }
}
