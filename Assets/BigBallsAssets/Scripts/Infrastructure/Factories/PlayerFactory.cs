using BigBalls.GameplayObjects;
using BigBalls.Providers;
using BigBalls.Services;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace BigBalls.Factories
{
    public class PlayerFactory
    {
        private readonly IInputService _inputService;
        private readonly IPlayerProvider _playerProvider;
        private readonly IObjectResolver _objectResolver;
        private readonly ISceneContainerProvider _sceneContainerProvider;
        private readonly IResourceLoader _resourceLoader;

        public PlayerFactory(
            IInputService inputService,
            IPlayerProvider playerProvider,
            IObjectResolver objectResolver,
            ISceneContainerProvider sceneContainerProvider,
            IResourceLoader resourceLoader)
        {
            _inputService = inputService;
            _playerProvider = playerProvider;
            _objectResolver = objectResolver;
            _resourceLoader = resourceLoader;
        }

        public Player Create()
        {
            Player player = _objectResolver.Instantiate(_resourceLoader.Load<Player>(), _sceneContainerProvider.PlayerSpawnPoint.position, Quaternion.identity);
            _playerProvider.Set(player);

            new Mover(player, player.transform);
            new Health(player);

            player.Construct(new PlayerAnimator());

            return player;

        }
    }
}
