using BigBalls.Configs;
using BigBalls.Factories;
using BigBalls.GameplayObjects;
using BigBalls.Infrastructure.DI;
using BigBalls.Services;
using BigBalls.UI;
using UnityEngine;
using VContainer.Unity;

namespace BigBalls.Infrastructure
{
    public class CreateLevelState : IPayloadedState<LevelID>
    {
        private readonly IObjectResolverProvider _objectResolverProvider;
        private readonly IPlayerFactory _playerFactory;
        private readonly IWindowService _windowService;
        private readonly IUpdateService _updateService;
        private readonly IUIFactory _uIFactory;
        private readonly ITimeService _timeService;
        private readonly IResourceLoader _resourceLoader;
        private readonly IPoolService _poolService;
        private readonly TileFactory _tileGenerator;
        private readonly TileMover _tileMover;
        private readonly EnemySpawner _enemySpawner;
        private readonly LevelTimeline _levelTimeline;
        private readonly IDropService _dropService;

        private LevelConfig _levelConfig;

        public CreateLevelState(
            IObjectResolverProvider objectResolverProvider,
            IPlayerFactory playerFactory,
            IWindowService windowService,
            IUpdateService updateService,
            IUIFactory uIFactory,
            ITimeService timeService,
            IResourceLoader resourceLoader,
            IPoolService poolService,
            ILevelLoadingService levelLoadingService,
            TileFactory tileGenerator,
            TileMover tileMover,
            EnemySpawner enemySpawner,
            LevelTimeline levelTimeline,
            IDropService dropService
            )
        {
            _objectResolverProvider = objectResolverProvider;
            _playerFactory = playerFactory;
            _windowService = windowService;
            _updateService = updateService;
            _uIFactory = uIFactory;
            _timeService = timeService;
            _resourceLoader = resourceLoader;
            _poolService = poolService;
            _tileGenerator = tileGenerator;
            _tileMover = tileMover;
            _enemySpawner = enemySpawner;
            _levelTimeline = levelTimeline;
            _dropService = dropService;
        }

        public void Enter(LevelID level)
        {
            _levelConfig = _resourceLoader.Load<LevelData>().Get(level);
            _poolService.SetCurrentLevelConfig(_levelConfig);
            _poolService.InitializePools();
            _windowService.CreateUIRoot();
            _windowService.Open<HUD>();
            _playerFactory.Create();

            _dropService.SetCurrentLevelConfig(_levelConfig);
            _tileGenerator.StartSpawn(_levelConfig);
            _tileMover.Start();
            _levelTimeline.Start(level);

            _objectResolverProvider.CurrentResolver.Instantiate(_resourceLoader.Load<Camera>());
        }

        public void Exit()
        {
            _levelTimeline.Stop();
            _uIFactory.ClearCache();
            _updateService.Clear();
            _timeService.ResumeGame();
            _poolService.ClearAllPools();
        }
    }
}