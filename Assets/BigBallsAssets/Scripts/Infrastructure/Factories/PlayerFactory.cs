using BigBalls.GameplayObjects;
using BigBalls.Providers;
using BigBalls.Services;
using BigBalls.StaticData;
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
        private readonly IPlayerProvider _playerProvider;
        private readonly IObjectResolver _objectResolver;
        private readonly ISceneContainerProvider _sceneContainerProvider;
        private readonly IResourceLoader _resourceLoader;
        private readonly IUpdateService _updateService;
        private readonly IIdentifierService _identifierService;

        private PlayerConfig _playerConfig;

        private List<Stat> _statHolder = new();

        public IReadOnlyList<Stat> Stats => _statHolder.AsReadOnly();

        public PlayerFactory(
            IInputService inputService,
            IPlayerProvider playerProvider,
            IObjectResolver objectResolver,
            ISceneContainerProvider sceneContainerProvider,
            IResourceLoader resourceLoader,
            IUpdateService updateService,
            IIdentifierService identifierService)
        {
            _inputService = inputService;
            _playerProvider = playerProvider;
            _objectResolver = objectResolver;
            _resourceLoader = resourceLoader;
            _sceneContainerProvider = sceneContainerProvider;
            _updateService = updateService;
            _identifierService = identifierService;

            _playerConfig = resourceLoader.Load<PlayerConfig>();
        }

        public Player Create()
        {
            Vector3 spawnPoint = _sceneContainerProvider.PlayerSpawnPoints.First().transform.position;
            Player prefab = _resourceLoader.Load<Player>();

            Player player = _objectResolver.Instantiate(prefab, spawnPoint, Quaternion.identity);
            int playerID = _identifierService.ID;

            player.Construct(playerID, new PlayerAnimator());

            StatHolder statHolder =  new StatHolder(playerID, _playerConfig);

            Mover mover = new Mover(statHolder.Get(StatType.MoveSpeed), player.transform, _updateService, _playerConfig);
            Rotator rotator = new Rotator(statHolder.Get(StatType.RotationSpeed), player.transform, _updateService);

            new Shooter(statHolder.Get(StatType.Damage), player.transform);
            new Health(statHolder.Get(StatType.Health));

            new PlayerMover(_inputService, rotator, mover);

            _playerProvider.Set(player);
            _playerProvider.Set(statHolder.Stats.ToList());

            return player;
        }
    }
}
