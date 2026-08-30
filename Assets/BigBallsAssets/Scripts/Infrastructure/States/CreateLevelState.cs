using BigBalls.Configs;
using BigBalls.Factories;
using BigBalls.GameplayObjects;
using BigBalls.Infrastructure.DI;
using BigBalls.Services;
using BigBalls.UI;
using log4net.Core;
using System.Collections.Generic;
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
        private readonly BallRepository _ballsRepository;
        private readonly ArtefactsRepository _artefactsRepository;
        private readonly IWinService _winService;
        private readonly ILouseService _louseService;
        private readonly ICameraProvider _cameraProvider;
        private readonly ISaveService _saveService;
        private readonly IInputService _inputService;
        private LevelConfig _levelConfig;
        private List<EnemyConfig> _currentEnemyConfig;
        private LevelID _currentLevel;
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
            LevelTimeline levelTimeline,
            IDropService dropService,
            BallRepository ballsRepository,
            ArtefactsRepository artefactsRepository,
            IWinService winService,
            ILouseService louseService,
            ICameraProvider cameraProvider,
            ISaveService saveService,
            IInputService inputService)
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
            _ballsRepository = ballsRepository;
            _artefactsRepository = artefactsRepository;
            _winService = winService;
            _louseService = louseService;
            _cameraProvider = cameraProvider;
            _saveService = saveService;
            _inputService = inputService;
        }

        public void Enter(LevelID level)
        {
            Debug.Log(level);
            _currentLevel = level;
            _windowService.CreateUIRoot();
            _windowService.Open<MainMenu>();
            _uIFactory.UIRoot.GlobalWalletView.Enable();

            _levelConfig = _resourceLoader.Load<LevelData>().Get(level);
            _currentEnemyConfig = _levelConfig.GetAllLevelEnemies();

            _poolService.SetCurrentLevelConfig(_currentEnemyConfig, _levelConfig);
            _poolService.InitializePools();

            Camera camera = CreateCamera();
            CameraOrbit cameraOrbit = camera.GetComponent<CameraOrbit>();
            cameraOrbit = camera.GetComponent<CameraOrbit>();
            Player player = _playerFactory.Create();
            cameraOrbit.StartOrbit(player.transform);
            _dropService.SetCurrentLevelConfig(_currentEnemyConfig);
        }

        public void StartLevel()
        {
            CameraOrbit cameraOrbit = _cameraProvider.Camera.GetComponent<CameraOrbit>();
            cameraOrbit.StopOrbit();
            RoadColliderPlacer colliderPlacer = _cameraProvider.Camera.GetComponent<RoadColliderPlacer>();
            CameraSmoothTransition cameraTransition = _cameraProvider.Camera.GetComponent<CameraSmoothTransition>();
            cameraTransition.MoveCamera(colliderPlacer.PlaceWalls);

            _uIFactory.Get<MainMenu>(WindowType.MainMenu).Close();
            _playerFactory.OnPlayerSpawned();
            _inputService.Start();
            _levelTimeline.Start(_levelConfig);
            _tileGenerator.StartSpawn(_levelConfig);
            _tileMover.Start();
            _winService.SetLevel(_currentLevel);
            _louseService.SetLevel(_currentLevel);
            CreateUI();
        }

        private Camera CreateCamera()
        {
            Camera prefab = _resourceLoader.Load<Camera>();
            Camera camera = _objectResolverProvider.CurrentResolver.Instantiate(prefab);
            _cameraProvider.Camera = camera;
            return camera;
        }

        private void CreateUI()
        {
            _windowService.CreateUIRoot();
            _windowService.Open<HUD>();
            _uIFactory.Get<HUD>(WindowType.HUD).WaveViewer.StartView(_levelConfig);
            _uIFactory.UIRoot.GlobalWalletView.Disable();
        }

        public void Exit()
        {
            _saveService.Save();
            _levelTimeline.Stop();
            _uIFactory.ClearCache();
            _updateService.Clear();
            _timeService.ResumeGame();
            _poolService.ClearAllPools();
            _ballsRepository.Save();


            _artefactsRepository.Save();
            _ballsRepository.Save();
            _saveService.Save();
            _uIFactory.ClearCache();
        }
    }
}