using BigBalls.GameplayObjects;
using BigBalls.Providers;
using BigBalls.Services;
using BigBalls.StaticData;
using BigBalls.UI;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace BigBalls.Factories
{
    public class PlayerFactory : IPlayerFactory
    {
        private readonly IInputService _inputService;
        private readonly IObjectResolver _objectResolver;
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

        private PlayerConfig _playerConfig;

        private List<Stat> _statHolder = new();

        public IReadOnlyList<Stat> Stats => _statHolder.AsReadOnly();

        public PlayerFactory(
            IInputService inputService,
            IObjectResolver objectResolver,
            ISceneContainerProvider sceneContainerProvider,
            IResourceLoader resourceLoader,
            IUpdateService updateService,
            IIdentifierService identifierService,
            IUIFactory uIFactory,
            IEntityRepository entityRepository,
            IRaycastService raycastService,
            IPlayerProvider playerProvider,
            IBallFactory ballFactory,
            ICoroutineRunner coroutineRunner)
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
            _playerConfig = resourceLoader.Load<PlayerConfig>();
        }

        public Player Create()
        {
            int playerID = _identifierService.ID;
            Vector3 spawnPoint = _sceneContainerProvider.PlayerSpawnPoints.First().transform.position;
            Player prefab = _resourceLoader.Load<Player>();
            Player player = _objectResolver.Instantiate(prefab, spawnPoint, Quaternion.identity);

            StatHolder statHolder = new StatHolder(playerID, _playerConfig);

            DeathHandler<Player> deathHandler = new DeathHandler<Player>(statHolder[StatType.Health], CreateComponents(statHolder, player), player);
            deathHandler.Subscribe();
            deathHandler.Died += OnDied;

            player.Construct(playerID, deathHandler);

            _uIFactory.Get<HUD>(WindowType.HUD).PlayerHealthViewer.Init(statHolder[StatType.Health]);
            _entityRepository.Add(player, statHolder);
            _playerProvider.Set(player);

            return player;
        }

        private void OnDied(Player player)
        {
            player.DeathHandler.Unsubscribe();
            player.DeathHandler.Died -= OnDied;
            player.gameObject.SetActive(false);
        }

        private List<ISubscribable> CreateComponents(StatHolder statHolder, Player player)
        {
            Mover mover = new Mover(statHolder[StatType.MoveSpeed], player.transform, _updateService, _raycastService, _playerConfig);
            Rotator rotator = new Rotator(statHolder[StatType.RotationSpeed], player.transform, _updateService);
            PlayerBallContainer playerBallContainer = new PlayerBallContainer(statHolder[StatType.BallBag], _resourceLoader, _ballFactory);
            Shooter shooter = new Shooter(statHolder[StatType.Damage], statHolder[StatType.AttackSpeed], player.transform, playerBallContainer, _coroutineRunner);
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
