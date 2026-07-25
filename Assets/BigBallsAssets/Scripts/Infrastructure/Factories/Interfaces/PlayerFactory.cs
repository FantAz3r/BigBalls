using BigBalls.Configs;
using BigBalls.GameplayObjects;
using BigBalls.Infrastructure.DI;
using BigBalls.Providers;
using BigBalls.Services;
using BigBalls.StaticData;
using BigBalls.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer.Unity;

namespace BigBalls.Factories
{
    public class PlayerFactory : IPlayerFactory
    {
        private readonly IInputService _inputService;
        private readonly IObjectResolverProvider _objectResolver;
        private readonly ISceneContainerProvider _sceneContainerProvider;
        private readonly IResourceLoader _resourceLoader;
        private readonly IUpdateService _updateService;
        private readonly IIdentifierService _identifierService;
        private readonly IUIFactory _uIFactory;
        private readonly IEntityRepository _entityRepository;
        private readonly IRaycastService _raycastService;
        private readonly IPlayerProvider _playerProvider;
        private readonly IBallFactory _ballFactory;
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly ILouseService _louseService;
        private readonly IItemConainerProvider _itemConainerProvider;
        private readonly IEffectFactory _effectFactory;
        private readonly IPlayerExperience _playerExperience;
        private readonly EnemySpawner _enemySpawner;
        private readonly BallRepository _ballRepository;
        private readonly ArtefactsRepository _artefactsRepository;
        private readonly ICameraProvider _cameraProvider;
        private PlayerConfig _playerConfig;

        public PlayerFactory(
            IInputService inputService,
            IObjectResolverProvider objectResolver,
            ISceneContainerProvider sceneContainerProvider,
            IResourceLoader resourceLoader,
            IUpdateService updateService,
            IIdentifierService identifierService,
            IUIFactory uIFactory,
            IEntityRepository entityRepository,
            IRaycastService raycastService,
            IPlayerProvider playerProvider,
            IBallFactory ballFactory,
            ICoroutineRunner coroutineRunner,
            ILouseService louseService,
            IItemConainerProvider itemConainerProvider,
            IEffectFactory effectFactory,
            IPlayerExperience playerExperience,
            EnemySpawner enemySpawner,
            BallRepository ballRepository,
            ArtefactsRepository artefactsRepository,
            ICameraProvider cameraProvider)
        {
            _inputService = inputService;
            _objectResolver = objectResolver;
            _resourceLoader = resourceLoader;
            _sceneContainerProvider = sceneContainerProvider;
            _updateService = updateService;
            _identifierService = identifierService;
            _uIFactory = uIFactory;
            _entityRepository = entityRepository;
            _raycastService = raycastService;
            _playerProvider = playerProvider;
            _ballFactory = ballFactory;
            _coroutineRunner = coroutineRunner;
            _louseService = louseService;
            _itemConainerProvider = itemConainerProvider;
            _effectFactory = effectFactory;
            _playerExperience = playerExperience;
            _enemySpawner = enemySpawner;
            _ballRepository = ballRepository;
            _artefactsRepository = artefactsRepository;
            _cameraProvider = cameraProvider;
            _playerConfig = resourceLoader.Load<PlayerConfig>();
        }

        public Player Create()
        {
            int playerID = _identifierService.ID;
            Vector3 spawnPoint = _sceneContainerProvider.PlayerSpawnPoints.First().transform.position;
            Player prefab = _resourceLoader.Load<Player>();
            Player player = _objectResolver.CurrentResolver.Instantiate(prefab, spawnPoint, Quaternion.identity);

            StatHolder statHolder = new StatHolder(player, EntityType.Player, _playerConfig, _effectFactory);
            statHolder.Set(_itemConainerProvider.Armor);
            statHolder.InitStats();

            _playerExperience.Init(statHolder[StatType.Experience]);

            ComponentContainer container = new ComponentContainer();

            List<ISubscribable> components = CreateComponents(out PlayerCardHolder cardHolder, statHolder, player);

            DeathHandler<Player> deathHandler = new DeathHandler<Player>(statHolder[StatType.Health], components, player);
            deathHandler.Subscribe();

            _louseService.SetLouseReason(deathHandler);
            player.Construct(playerID, deathHandler);

            player.EventHandler.Died += OnDied;

            _uIFactory.Get<HUD>(WindowType.HUD).PlayerHealthViewer.Init(statHolder[StatType.Health]);
            _uIFactory.Get<HUD>(WindowType.HUD).PlayerExperienceViewer.Init(_playerExperience);
            _uIFactory.Get<CardSelectionMenu>(WindowType.CardMenu).Init(cardHolder);
            _uIFactory.Get<HUD>(WindowType.HUD).PlayerSlots.Init(cardHolder);

            foreach (var component in components)
            {
                container.Add(component);
            }

            _entityRepository.Add(player, statHolder, container);
            _playerProvider.Set(player);

            return player;
        }

        private void OnDied(IEntity entity)
        {
            if (entity is not Player player)
                return;

            player.DeathHandler.Unsubscribe();
            player.EventHandler.Died -= OnDied;
            player.gameObject.SetActive(false);
        }

        private List<ISubscribable> CreateComponents(out PlayerCardHolder cardHolder, StatHolder statHolder, Player player)
        {
            MoverPhythics mover = new MoverPhythics(statHolder[StatType.MoveSpeed], player.transform, _updateService, player.Rigidbody , _raycastService, _playerConfig);
            Rotator rotator = new Rotator(statHolder[StatType.RotationSpeed], player.transform, _updateService);

            PlayerBallContainer playerBallContainer = new PlayerBallContainer(statHolder[StatType.BallBag], _resourceLoader, _ballFactory, _effectFactory, _identifierService, _ballRepository);
            ArtefactContainer artefactContainer = new ArtefactContainer(_effectFactory, _enemySpawner, playerBallContainer, statHolder, _artefactsRepository);

            cardHolder = new PlayerCardHolder(playerBallContainer, artefactContainer);
            cardHolder.AddItem(_itemConainerProvider.Helmet);
            cardHolder.AddItem(_itemConainerProvider.Gun);

            if (_itemConainerProvider.Gun != null)
            {
                Cannon cannon = GameObject.Instantiate(_itemConainerProvider.Gun.WeaponConfig.Cannon, player.WeaponSpawnPoint);
                player.SetFirePoint(cannon);
            }

            Shooter shooter = new Shooter(statHolder[StatType.AttackSpeed], player.WeaponSpawnPoint, playerBallContainer);
            _objectResolver.CurrentResolver.Inject(shooter);
            shooter.StartShoot();

            player.SquashOnShot.Init(shooter);
            player.TiltOnShot.Init(shooter);

            ScreenShake screenShake = _cameraProvider.Camera.GetComponent<ScreenShake>();
            screenShake.Init(shooter, statHolder[StatType.Health]);

            PlayerMover playerMover = new PlayerMover(_inputService, rotator, mover);
            HealthRegenerator healthRegenerator = new HealthRegenerator(statHolder[StatType.Health], statHolder[StatType.HealthRegen], _updateService);

            List<ISubscribable> subscribables = new List<ISubscribable>()
            {
                mover,
                shooter,
                rotator,
                playerMover,
                healthRegenerator,
                playerBallContainer,
            };

            return subscribables;
        }
    }
}
