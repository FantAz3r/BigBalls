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
            IPlayerProvider playerProvider)
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
            _playerConfig = resourceLoader.Load<PlayerConfig>();
        }

        public Player Create()
        {
            Vector3 spawnPoint = _sceneContainerProvider.PlayerSpawnPoints.First().transform.position;
            Player prefab = _resourceLoader.Load<Player>();

            Player player = _objectResolver.Instantiate(prefab, spawnPoint, Quaternion.identity);
            int playerID = _identifierService.ID;

            StatHolder statHolder =  new StatHolder(playerID, _playerConfig);

            Mover mover = new Mover(statHolder[StatType.MoveSpeed], player.transform, _updateService, _playerConfig, _raycastService);
            Rotator rotator = new Rotator(statHolder[StatType.RotationSpeed], player.transform, _updateService);
            Shooter shooter = new Shooter(statHolder[StatType.Damage], player.transform);
            PlayerMover playerMover =   new PlayerMover(_inputService, rotator, mover);
            HealthRegenerator healthRegenerator = new HealthRegenerator(statHolder[StatType.Health], statHolder[StatType.HealthRegen], _updateService);

            List<ISubscribable> subscribables = new List<ISubscribable>()
            {
                mover,
                shooter,
                rotator,
                playerMover,
                healthRegenerator
            };

            DeathHandler playerDeathHandler = new DeathHandler( statHolder[StatType.Health], subscribables);
            player.Construct(playerID, playerDeathHandler);

            _uIFactory.Get<HUD>(WindowType.HUD).PlayerHealthViewer.Init(statHolder[StatType.Health]);
            _entityRepository.Add(player, statHolder);
            _playerProvider.Set(player);

            return player;
        }
    }
}
