using System.Collections.Generic;
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
        private readonly LevelTimeline _levelTimeline;
        private readonly IDropService _dropService;

        private LevelConfig _levelConfig;
        private List<EnemyConfig> _currentEnemyConfig;

        public CreateLevelState (
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
            _levelTimeline = levelTimeline;
            _dropService = dropService;
        }

        public void Enter (LevelID level)
        {
            _levelConfig = _resourceLoader.Load<LevelData>().Get(level);
            _currentEnemyConfig = _levelConfig.GetCurrentEnemyConfigToLevel();
            _poolService.SetCurrentLevelConfig(_currentEnemyConfig, _levelConfig);
            _poolService.InitializePools();

            CreateUI();

            _playerFactory.Create();

            _dropService.SetCurrentLevelConfig(_currentEnemyConfig);
            _tileGenerator.StartSpawn(_levelConfig);
            _tileMover.Start();
            _levelTimeline.Start(_levelConfig);

            _objectResolverProvider.CurrentResolver.Instantiate(_resourceLoader.Load<Camera>());
        }

        private void CreateUI ()
        {
            _windowService.CreateUIRoot();
            _windowService.Open<HUD>();
            _uIFactory.Get<HUD>(WindowType.HUD).WaveViewer.StartView(_levelConfig);
        }

        public void Exit ()
        {
            _levelTimeline.Stop();
            _uIFactory.ClearCache();
            _updateService.Clear();
            _timeService.ResumeGame();
            _poolService.ClearAllPools();
        }
    }
}