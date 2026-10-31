using System.Collections.Generic;
using System.Linq;
using BigBalls.GameplayObjects;
using BigBalls.Infrastructure.DI;
using BigBalls.Providers;
using BigBalls.Services;
using BigBalls.StaticData;
using BigBalls.UI;
using UnityEngine;
using VContainer.Unity;

namespace BigBalls.Factories
{
    public class PlayerFactory : IPlayerFactory
    {
        private readonly IObjectResolverProvider _objectResolver;
        private readonly ISceneContainerProvider _sceneContainerProvider;
        private readonly IResourceLoader _resourceLoader;
        private readonly IUpdateService _updateService;
        private readonly IIdentifierService _identifierService;
        private readonly IUIFactory _uIFactory;
        private readonly IEntityRepository _entityRepository;
        private readonly IPlayerProvider _playerProvider;
        private readonly ILouseService _louseService;
        private readonly IItemConainerProvider _itemConainerProvider;
        private readonly IEffectFactory _effectFactory;
        private readonly IPlayerExperience _playerExperience;
        private readonly EnemySpawner _enemySpawner;
        private readonly ArtefactsRepository _artefactsRepository;
        private readonly ICameraProvider _cameraProvider;
        private readonly IMergeService _mergeService;

        private PlayerConfig _playerConfig;
        private PlayerCardHolder _cardHolder;
        private Shooter _shooter;
        private Player _player;

        public PlayerFactory (
            IObjectResolverProvider objectResolver,
            ISceneContainerProvider sceneContainerProvider,
            IResourceLoader resourceLoader,
            IUpdateService updateService,
            IIdentifierService identifierService,
            IUIFactory uIFactory,
            IEntityRepository entityRepository,
            IPlayerProvider playerProvider,
            ILouseService louseService,
            IItemConainerProvider itemConainerProvider,
            IEffectFactory effectFactory,
            IPlayerExperience playerExperience,
            EnemySpawner enemySpawner,
            ArtefactsRepository artefactsRepository,
            ICameraProvider cameraProvider,
            IMergeService mergeService)
        {
            _objectResolver = objectResolver;
            _resourceLoader = resourceLoader;
            _sceneContainerProvider = sceneContainerProvider;
            _updateService = updateService;
            _identifierService = identifierService;
            _uIFactory = uIFactory;
            _entityRepository = entityRepository;
            _playerProvider = playerProvider;
            _louseService = louseService;
            _itemConainerProvider = itemConainerProvider;
            _effectFactory = effectFactory;
            _playerExperience = playerExperience;
            _enemySpawner = enemySpawner;
            _artefactsRepository = artefactsRepository;
            _cameraProvider = cameraProvider;
            _mergeService = mergeService;
            _playerConfig = resourceLoader.Load<PlayerConfig>();
        }

        public Player Create ()
        {
            int playerID = _identifierService.ID;
            Vector3 spawnPoint = _sceneContainerProvider.PlayerSpawnPoints.First().transform.position;
            Player prefab = _resourceLoader.Load<Player>();
            _player = _objectResolver.CurrentResolver.Instantiate(prefab, spawnPoint, Quaternion.identity);

            StatHolder statHolder = new StatHolder(_player, EntityType.Player, _playerConfig, _effectFactory);
            statHolder.InitStats();
            statHolder.Set(_itemConainerProvider.Armor);
            _player.ArmorViewer.Init(statHolder);

            _playerExperience.Init(statHolder[StatType.Experience]);

            ComponentContainer container = new ComponentContainer();

            List<ISubscribable> components = CreateComponents(statHolder, _player);

            HealthHandler<Player> deathHandler = new HealthHandler<Player>(statHolder[StatType.Health], components, _player);
            deathHandler.Subscribe();
            DeathSubscribe();

            _louseService.SetLouseReason(deathHandler);
            _player.Construct(playerID, deathHandler);

            _uIFactory.Get<HUD>(WindowType.HUD).PlayerHealthViewer.Init(statHolder[StatType.Health]);
            _uIFactory.Get<HUD>(WindowType.HUD).PlayerExperienceViewer.Init(_playerExperience);
            _uIFactory.Get<CardSelectionMenu>(WindowType.CardMenu).Init(_cardHolder);
            _uIFactory.Get<HUD>(WindowType.HUD).PlayerSlots.Init(_cardHolder);

            foreach (var component in components)
            {
                container.Add(component);
            }

            _entityRepository.Add(_player, statHolder, container);
            _playerProvider.Set(_player);

            return _player;
        }

        public void DeathSubscribe() => _player.EventHandler.Died += OnDied;

        private void OnDied (IEntity entity)
        {
            if (entity is not Player player)
                return;

            player.DeathHandler.Unsubscribe();
            player.EventHandler.Died -= OnDied;
            player.gameObject.SetActive(false);
        }

        private List<ISubscribable> CreateComponents (StatHolder statHolder, Player player)
        {
            MoverPhythics mover = new MoverPhythics(statHolder[StatType.MoveSpeed], player.transform, player.Rigidbody, _playerConfig);
            _objectResolver.CurrentResolver.Inject(mover);

            Rotator rotator = new Rotator(statHolder[StatType.RotationSpeed], player.transform);
            _objectResolver.CurrentResolver.Inject(rotator);

            PlayerBallContainer playerBallContainer = new PlayerBallContainer(statHolder[StatType.BallBag]);
            _objectResolver.CurrentResolver.Inject(playerBallContainer);

            ArtefactContainer artefactContainer = new ArtefactContainer(_effectFactory, _enemySpawner, playerBallContainer, statHolder, _artefactsRepository);

            _cardHolder = new PlayerCardHolder(playerBallContainer, artefactContainer, _itemConainerProvider);
            _mergeService.Init(_cardHolder);

            if (statHolder.TryGetStat(out Stat collectRange, StatType.CollectRange))
            {
                player.ResourceCollector.Init(collectRange);
            }

            SplitingPanel splitingPanel = _uIFactory.Get<SplitingPanel>(WindowType.MergeWindow);
            splitingPanel.MergeWindow.Init(_cardHolder);
            splitingPanel.SplitingWindow.Init(_cardHolder);

            _shooter = new Shooter(
                statHolder[StatType.AttackSpeed],
                statHolder[StatType.Accuracy],
                player.WeaponViewer.CurrentCannon,
                playerBallContainer);

            _objectResolver.CurrentResolver.Inject(_shooter);

            player.WeaponViewer.Init(_shooter, _cardHolder);
            player.SquashOnShot.Init(_shooter);
            player.TiltOnShot.Init(_shooter);

            ScreenShake screenShake = _cameraProvider.Camera.GetComponent<ScreenShake>();
            screenShake.Init(_shooter, statHolder[StatType.Health]);

            PlayerMover playerMover = new PlayerMover(rotator, mover);
            _objectResolver.CurrentResolver.Inject(playerMover);

            HealthRegenerator healthRegenerator = new HealthRegenerator(statHolder[StatType.Health], statHolder[StatType.HealthRegen], _updateService);

            List<ISubscribable> subscribables = new List<ISubscribable>()
            {
                mover,
                _shooter,
                rotator,
                playerMover,
                healthRegenerator,
                playerBallContainer,
            };

            return subscribables;
        }

        public void OnPlayerSpawned ()
        {
            _shooter.StartShoot();
            _cardHolder.Init();
        }
    }
}
