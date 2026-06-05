using BigBalls.Services;
using UnityEngine;


namespace BigBalls.GameplayObjects
{
    public class EnemyAttacker: ISubscribable
    {
        private readonly IPlayerProvider _playerProvider;
        private readonly IDamageService _damageService;
        private readonly Stat _damage;
        private readonly Transform _owner;
        private readonly EntityTrigger _entityTrigger;

        public EnemyAttacker(IPlayerProvider playerProvider, Stat damage, Transform owner, EntityTrigger entityTrigger , IDamageService damageService)
        {
            _playerProvider = playerProvider;
            _damage = damage;
            _owner = owner;
            _entityTrigger = entityTrigger;
            _damageService = damageService;
        }

        public void Subscribe()
        {
            _entityTrigger.WallDetected += Attack;
            _entityTrigger.PlayerDetected += Attack;
        }

        public void Unsubscribe()
        {
            _entityTrigger.PlayerDetected -= Attack;
            _entityTrigger.WallDetected -= Attack;
        }

        private void Attack()
        {

            _damageService.ApplyDamage(_playerProvider.Player.Id, _damage.CurrentValue);
            GameObject.Destroy(_owner.gameObject);
        }
    }
}