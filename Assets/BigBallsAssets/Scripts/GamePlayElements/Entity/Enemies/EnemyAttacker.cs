using BigBalls.Configs;
using BigBalls.Factories;
using BigBalls.Services;
using DG.Tweening;
using System.Collections;
using UnityEngine;
using VContainer;

namespace BigBalls.GameplayObjects
{
    public class EnemyAttacker : ISubscribable
    {
        private readonly EnemyConfig _config;
        private readonly Stat _damage;
        private readonly Enemy _owner;
        private readonly EntityTrigger _entityTrigger;
        private readonly Shooter _shooter;

        private IEntityRepository _entityRepository;
        private IPlayerProvider _playerProvider;
        private IDamageService _damageService;
        private ICoroutineRunner _coroutineRunner;
        private IBallFactory _ballFactory;
        private IIdentifierService _identifierService;
        private bool _canAttack;

        public EnemyAttacker(Stat damage, Enemy owner, EnemyConfig config, Shooter shooter)
        {
            _damage = damage;
            _owner = owner;
            _entityTrigger = owner.EntityTrigger;
            _config = config;
            _shooter = shooter;
        }

        [Inject]
        public void Construst(
            IPlayerProvider playerProvider,
            IDamageService damageService,
            ICoroutineRunner coroutineRunner,
            IBallFactory ballFactory,
            IIdentifierService identifierService)
        {
            _coroutineRunner = coroutineRunner;
            _playerProvider = playerProvider;
            _damageService = damageService;
            _identifierService = identifierService;
            _ballFactory = ballFactory;
        }

        public void Subscribe()
        {
            _canAttack = true;
            _entityTrigger.WallDetected += MeeleAttack;
            _entityTrigger.PlayerStayedLongEnough += MeeleAttack;
            _entityTrigger.PlayerCollision += CollisionAttack;
            RangeAttack();
        }

        public void Unsubscribe()
        {
            _entityTrigger.PlayerStayedLongEnough -= MeeleAttack;
            _entityTrigger.WallDetected -= MeeleAttack;
            _entityTrigger.PlayerCollision -= CollisionAttack;
        }

        private void CollisionAttack()
        {
            _damageService.ApplyDamage(_playerProvider.Player, _damage.CurrentValue);
        }

        private void MeeleAttack()
        {
            if (_config.HasMeeleAttack == false)
                return;

            _coroutineRunner.StartCoroutine(MeeleAttackRoutine(_config.Type == EntityType.Enemy));
        }

        private void RangeAttack()
        {
            if (_config.HasRangeAttack == false)
                return;

            BallModel ballModel = new BallModel(_identifierService.ID, _config.BallConfig);
            Ball ball = _ballFactory.Create(ballModel, EntityType.Enemy);
            _shooter.StartShoot(ball);
        }

        private IEnumerator MeeleAttackRoutine(bool needSuiside)
        {
            _canAttack = false;
            Vector3 startPosition = _owner.transform.position;
            Vector3 playerPosition = _playerProvider.Player.transform.position;
            Vector3 targetPosition = playerPosition;

            float jumpPower = 3f;
            float jumpDuration = 0.4f;
            float returnDuration = 0.2f;

            Sequence attackSequence = DOTween.Sequence();

            attackSequence.Append(
                _owner.transform.DOJump(targetPosition, jumpPower, 1, jumpDuration)
            );

            attackSequence.AppendCallback(() =>
            {
                ApplyMeeleDamage(needSuiside);
            });

            attackSequence.Append(
                _owner.transform.DOMove(startPosition, returnDuration).SetEase(Ease.InOutQuad)
            );

            yield return attackSequence.WaitForCompletion();

            _canAttack = true;
        }

        private void ApplyMeeleDamage(bool needSuiside)
        {
            var player = _playerProvider.Player;
            _damageService.ApplyDamage(player, _damage.CurrentValue);
            PlayerKnockBack();

            if (needSuiside)
                _owner.EventHandler.Suiside(_owner);
        }

        private void PlayerKnockBack()
        {
            MoverPhythics mover = _entityRepository.GetContainer(_playerProvider.Player).Get<MoverPhythics>();
            Vector3 pushDirection = (_playerProvider.Player.transform.position - _owner.transform.position).normalized;
            mover.AddInstantPush(pushDirection);
        }
    }
}