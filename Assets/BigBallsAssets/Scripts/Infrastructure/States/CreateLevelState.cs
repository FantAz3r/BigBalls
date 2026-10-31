using System.Collections.Generic;
using BigBalls.Configs;
using BigBalls.Factories;
using BigBalls.GameplayObjects;
using BigBalls.Infrastructure.DI;
using BigBalls.Providers;
using BigBalls.Services;
using BigBalls.UI;
using Unity.VisualScripting;
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
        private readonly WeaponRepository _weaponRepository;
        private readonly ArmorRepository _armorRepository;
        private readonly HelmetRepository _helmetRepository;
        private readonly IWinService _winService;
        private readonly ILouseService _louseService;
        private readonly ICameraProvider _cameraProvider;
        private readonly ISaveService _saveService;
        private readonly IInputService _inputService;
        private readonly ISceneContainerProvider _sceneContainerProvider;
        private readonly IWalletModel _walletModel;
        private readonly ISoundService _soundService;
        private readonly SoundSpawner _soundSpawner;
        private readonly ILootFactory _lootFactory;
        private readonly IDeviceService _deviceService;

        private LevelConfig _levelConfig;
        private List<EnemyConfig> _currentEnemyConfigs;

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
            IDropService dropService,
            BallRepository ballsRepository,
            ArtefactsRepository artefactsRepository,
            WeaponRepository weaponRepository,
            ArmorRepository armorRepository,
            HelmetRepository helmetRepository,
            IWinService winService,
            ILouseService louseService,
            ICameraProvider cameraProvider,
            ISaveService saveService,
            InputServiceAdapter inputServiceAdapter,
            ISceneContainerProvider sceneContainerProvider,
            IWalletModel walletModel,
            ISoundService soundService,
            SoundSpawner soundSpawner,
            ILootFactory lootFactory,
            IDeviceService deviceService)
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
            _weaponRepository = weaponRepository;
            _armorRepository = armorRepository;
            _helmetRepository = helmetRepository;

            _winService = winService;
            _louseService = louseService;
            _cameraProvider = cameraProvider;
            _saveService = saveService;
            _inputService = inputServiceAdapter.Get();
            _sceneContainerProvider = sceneContainerProvider;
            _walletModel = walletModel;
            _soundService = soundService;
            _soundSpawner = soundSpawner;
            _lootFactory = lootFactory;
            _deviceService = deviceService;
        }

        public bool IsGameStarted { get; private set; }
        public LevelID CurrentLevel { get; private set; }

        public void Enter (LevelID level)
        {
            CurrentLevel = level;
            _saveService.GameProgress.Level = (int) level;
            _windowService.CreateUIRoot();
            _windowService.Open<MainMenu>();

            _uIFactory.UIRoot.GlobalWalletView.Enable();

            _levelConfig = _resourceLoader.Load<LevelData>().Get(level);
            _currentEnemyConfigs = _levelConfig.GetAllLevelEnemies();
            _tileGenerator.StartSpawn(_levelConfig);

            _poolService.SetCurrentLevelConfig(_currentEnemyConfigs, _levelConfig);
            _poolService.InitializePools();

            Camera camera = CreateCamera();
            CameraOrbit cameraOrbit = camera.GetComponent<CameraOrbit>();
            cameraOrbit = camera.GetComponent<CameraOrbit>();
            Player player = _playerFactory.Create();
            cameraOrbit.StartOrbit(player.transform);
            _dropService.SetCurrentLevelConfig(_currentEnemyConfigs);

            _uIFactory.Get<CardInventory>(WindowType.Inventory);
            _uIFactory.Get<SettingsView>(WindowType.Settings);

            _soundSpawner.Enable();
        }

        public void StartLevel ()
        {
            CameraOrbit cameraOrbit = _cameraProvider.Camera.GetComponent<CameraOrbit>();
            cameraOrbit.StopOrbit();
            CameraSmoothTransition cameraTransition = _cameraProvider.Camera.GetComponent<CameraSmoothTransition>();
            cameraTransition.MoveCamera(OnCameraReplaced);

            _sceneContainerProvider.BackgroundLooper.StartMove();
            _soundSpawner.Spawn(SoundType.StartGame);
            _uIFactory.Get<MainMenu>(WindowType.MainMenu).Close();
            _uIFactory.UIRoot.GlobalWalletView.Disable();
            _levelTimeline.Start(_levelConfig);
            _winService.SetLevel(CurrentLevel);
            _louseService.SetLevel(CurrentLevel);
            CreateGameUI();
            IsGameStarted = true;
        }

        private void OnCameraReplaced ()
        {
            RoadColliderPlacer colliderPlacer = _cameraProvider.Camera.GetComponent<RoadColliderPlacer>();
            colliderPlacer.PlaceWalls();

            if (_deviceService.IsDecktop == false)
            {
                _windowService.CreateJoystick(_inputService);
            }

            _tileMover.Start();

            _inputService.Start();
            _inputService.EnableUI();
            _inputService.EnablePlayer();

            _playerFactory.OnPlayerSpawned();
        }

        private Camera CreateCamera ()
        {
            Camera prefab = _resourceLoader.Load<Camera>();
            Camera camera = _objectResolverProvider.CurrentResolver.Instantiate(prefab);
            _cameraProvider.Camera = camera;
            return camera;
        }

        private void CreateGameUI ()
        {
            _windowService.Open<HUD>();
            _uIFactory.Get<HUD>(WindowType.HUD).WaveViewer.StartView(_levelConfig);
            _uIFactory.UIRoot.GlobalWalletView.Disable();
        }

        public void Exit ()
        {
            IsGameStarted = false;

            _inputService.Dispose();
            _lootFactory.Disable();
            _tileMover.Stop();
            _levelTimeline.Stop();
            _updateService.Clear();
            _soundService.StopAll();
            _soundSpawner.Disable();
            _poolService.ClearAllPools();
            _walletModel.PutAccumulatedCoins();

            _ballsRepository.Save();
            _artefactsRepository.Save();
            _weaponRepository.Save();
            _armorRepository.Save();
            _helmetRepository.Save();
            _saveService.Save();

            _uIFactory.ClearCache();
            _timeService.SetTimescale(1);
        }
    }
}