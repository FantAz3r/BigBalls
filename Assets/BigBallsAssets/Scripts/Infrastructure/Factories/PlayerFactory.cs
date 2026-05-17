using BigBalls.GameplayObjects;
using BigBalls.Providers;
using BigBalls.Services;
using System.Linq;
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

        public PlayerFactory(
            IInputService inputService,
            IPlayerProvider playerProvider,
            IObjectResolver objectResolver,
            ISceneContainerProvider sceneContainerProvider,
            IResourceLoader resourceLoader,
            IUpdateService updateService)
        {
            _inputService = inputService;
            _playerProvider = playerProvider;
            _objectResolver = objectResolver;
            _resourceLoader = resourceLoader;
            _sceneContainerProvider = sceneContainerProvider;
            _updateService = updateService;
        }

        public Player Create()
        {
            Vector3 spawnPoint = _sceneContainerProvider.PlayerSpawnPoints.First().transform.position;
            Player prefab = _resourceLoader.Load<Player>();

            Player player = _objectResolver.Instantiate(prefab, spawnPoint, Quaternion.identity);
            player.Construct(new PlayerAnimator());

            Mover mover = new Mover(player, player.transform, _updateService);
            Rotator rotator = new Rotator(player, player.transform, _updateService);


            new PlayerMover(_inputService, rotator, mover);
            new Health(player);

            _playerProvider.Set(player);
            return player;
        }
    }
}
