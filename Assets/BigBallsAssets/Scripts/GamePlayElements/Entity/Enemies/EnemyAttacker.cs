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
        private readonly EntityCollision _entityCollision;

        private IEntityRepository _entityRepository;
        private IPlayerProvider _playerProvider;
        private IDamageService _damageService;
        private ICoroutineRunner _coroutineRunner;
        private bool _canAttack;

        public EnemyAttacker(Stat damage, Enemy owner, EnemyConfig config)
        {
            _damage = damage;
            _owner = owner;
            _entityTrigger = owner.EntityTrigger;
            _entityCollision = owner.EntityCollision;
            _config = config;
        }

        [Inject]
        public void Construst(
            IPlayerProvider playerProvider,
            IDamageService damageService,
            ICoroutineRunner coroutineRunner,
            IEntityRepository entityRepository)
        {
            _coroutineRunner = coroutineRunner;
            _playerProvider = playerProvider;
            _damageService = damageService;
            _entityRepository = entityRepository;
        }

        public void Subscribe()
        {
            _canAttack = true;
            _entityTrigger.WallDetected += JumpAttack;
            _entityTrigger.PlayerStayedLongEnough += JumpAttack;

            if(_entityCollision != null)
                _entityCollision.PlayerCollision += CollisionAttack;
        }

        public void Unsubscribe()
        {
            _entityTrigger.PlayerStayedLongEnough -= JumpAttack;
            _entityTrigger.WallDetected -= JumpAttack;

            if (_entityCollision != null)
                _entityCollision.PlayerCollision -= CollisionAttack;
        }

        private void CollisionAttack()
        {
            _damageService.ApplyDamage(_playerProvider.Player, _damage.CurrentValue);
            PlayerKnockBack();
        }

        private void JumpAttack()
        {
            if (_config.HasMeeleAttack == false)
                return;

            _coroutineRunner.StartCoroutine(JumpAttackRoutine(_config.CanSuiside));
        }

        private IEnumerator JumpAttackRoutine(bool needSuiside)
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

            if (needSuiside)
                _owner.EventHandler.Suiside(_owner);
        }

        private void PlayerKnockBack()
        {
            MoverPhythics mover = _entityRepository.GetContainer(_playerProvider.Player).Get<MoverPhythics>();

            Vector3 rawDir = (_playerProvider.Player.transform.position - _owner.transform.position).normalized;
            Vector3 pushDirection = Mathf.Abs(rawDir.x) > Mathf.Abs(rawDir.z)
                ? new Vector3(Mathf.Sign(rawDir.x), 0, 0)
                : new Vector3(0, 0, Mathf.Sign(rawDir.z));

            mover.AddPush(pushDirection, 25);
        }
    }
}