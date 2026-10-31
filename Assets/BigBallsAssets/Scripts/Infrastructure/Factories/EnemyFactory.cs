using System;
using System.Collections.Generic;
using BigBalls.Configs;
using BigBalls.GameplayObjects;
using BigBalls.Infrastructure.DI;
using BigBalls.Services;
using UnityEngine;

namespace BigBalls.Factories
{
    public class EnemyFactory : IEnemyFactory
    {
        private readonly IObjectResolverProvider _resolverProvider;
        private readonly IIdentifierService _identifierService;
        private readonly IUpdateService _updateService;
        private readonly IRaycastService _raycastService;
        private readonly IPlayerProvider _playerProvider;
        private readonly IDamageService _damageService;
        private readonly IEntityRepository _entityRepository;
        private readonly IPoolService _poolService;
        private readonly IDropService _dropService;

        public EnemyFactory (
            IObjectResolverProvider resolverProvider,
            IIdentifierService identifierService,
            IUpdateService updateService,
            IRaycastService raycastService,
            IPlayerProvider playerProvider,
            IDamageService damageService,
            IEntityRepository entityRepository,
            IPoolService poolService,
            IDropService dropService
            )
        {
            _resolverProvider = resolverProvider;
            _identifierService = identifierService;
            _updateService = updateService;
            _raycastService = raycastService;
            _playerProvider = playerProvider;
            _damageService = damageService;
            _entityRepository = entityRepository;
            _poolService = poolService;
            _dropService = dropService;
        }

        public event Action<IEntity> Died;
        public event Action<IEntity> Killed;

        public int KillCount { get; private set; }
        public int DiedCount { get; private set; }
        public int SuisidedCount { get; private set; }

        public Enemy Create (EnemyConfig enemyConfig, Vector3 spawnPosition)
        {
            Enemy enemy = _poolService.Get(enemyConfig.Prefab, spawnPosition, enemyConfig.Prefab.transform.rotation);
            int enemyID = _identifierService.ID;

            StatHolder statHolder = new StatHolder(enemy, enemyConfig.Type, enemyConfig);
            statHolder.InitStats();

            List<ISubscribable> subscribables = CreateComponents(statHolder, enemy, enemyConfig);

            HealthHandler<Enemy> deathHandler = new HealthHandler<Enemy>(statHolder[StatType.Health], subscribables, enemy);
            deathHandler.Subscribe();
            enemy.EventHandler.Died += OnDied;

            Rotator enemyRotator = null;
            Mover enemyMover = null;
            EnemyShooter enemyShooter = null;

            foreach (var subscriber in subscribables)
            {
                if (subscriber is Mover mover)
                    enemyMover = mover;

                if (subscriber is Rotator rotator)
                    enemyRotator = rotator;

                if (subscriber is EnemyShooter shooter)
                    enemyShooter = shooter;
            }

            enemy.Init(enemyID, deathHandler, enemyConfig, statHolder, enemyRotator, enemyMover, enemyShooter);
            _entityRepository.Add(enemy, statHolder, null);

            enemy.ColorChanger?.Init(statHolder[StatType.Health]);
            enemy.ShieldView?.Init(statHolder[StatType.Shield]);

            return enemy;
        }

        private void OnSuisided (IEntity entity)
        {
            if (entity is not Enemy enemy)
                return;

            enemy.EventHandler.Suisided -= OnSuisided;
            SuisidedCount++;

            OnDied(entity);
        }

        private void OnDied (IEntity entity)
        {
            if (entity is not Enemy enemy)
                return;

            DiedCount++;
            enemy.DeathHandler.Unsubscribe();
            enemy.EventHandler.Died -= OnDied;
            _entityRepository.Remove(enemy);

            if (enemy.DieAnimation != null)
            {
                enemy.DieAnimation.Play(enemy, enemy => ReleaseToPool(enemy));
                return;
            }

            ReleaseToPool(enemy);
        }

        private void ReleaseToPool (Enemy enemy)
        {
            if (enemy.DeathHandler.IsBallAttacked)
            {
                KillCount++;
                _dropService.DropLoot(enemy);
                Killed?.Invoke(enemy);
            }

            _poolService.Release(enemy);
            Died?.Invoke(enemy);
        }

        private List<ISubscribable> CreateComponents (StatHolder statHolder, Enemy enemy, EnemyConfig enemyConfig)
        {
            Mover mover = null;
            EnemyShooter shooter = null;
            Rotator rotator = null;
            EnemyAttacker enemyAttacker = null;

            if (statHolder.TryGetStat(out Stat moveSpeed, StatType.MoveSpeed))
            {
                mover = new Mover(statHolder[StatType.MoveSpeed], enemy.transform, true, enemyConfig);
                _resolverProvider.CurrentResolver.Inject(mover);
                Init(mover, enemyConfig);
            }

            if (enemyConfig.HasRangeAttack)
            {
                shooter = new EnemyShooter(statHolder[StatType.AttackSpeed], enemy, enemyConfig, _playerProvider.Player.Transform);
                _resolverProvider.CurrentResolver.Inject(shooter);
            }

            if (statHolder.TryGetStat(out Stat rotationSpeed, StatType.RotationSpeed) && enemy.RotationPart != null)
            {
                rotator = new Rotator(rotationSpeed, enemy.RotationPart);
                _resolverProvider.CurrentResolver.Inject(rotator);
                rotator.SetTarget(_playerProvider.Player.transform);
            }

            if (statHolder.TryGetStat(out Stat damage, StatType.Damage))
            {
                enemyAttacker = new EnemyAttacker(damage, enemy, enemyConfig);
                _resolverProvider.CurrentResolver.Inject(enemyAttacker);
            }

            enemy.EventHandler.Suisided += OnDied;

            List<ISubscribable> subscribables = new List<ISubscribable>()
            {
                mover,
                enemyAttacker,
                shooter,
                rotator
            };

            return subscribables;
        }

        private void Init (Mover mover, EnemyConfig enemyConfig)
        {
            mover.SetDirection(new Vector2(0, -1));

            List<Vector2Int> frontRowBlocks = enemyConfig.GetBlocksWithoutFrontNeighbor();

            Vector3[] raycastPoints = new Vector3[frontRowBlocks.Count];
            Vector3[] raycastDirections = new Vector3[frontRowBlocks.Count];

            for (int i = 0; i < frontRowBlocks.Count; i++)
            {
                Vector2Int blockPos = frontRowBlocks[i];
                raycastPoints[i] = new Vector3(blockPos.x, 0.25f, blockPos.y);
                raycastDirections[i] = Vector3.back;
            }

            mover.SetReycastInfo(raycastDirections, raycastPoints);
        }
    }
}
