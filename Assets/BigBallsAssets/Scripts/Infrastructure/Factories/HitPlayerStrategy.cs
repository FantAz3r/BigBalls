using BigBalls.GameplayObjects;
using BigBalls.Services;
using UnityEngine;
using VContainer;

namespace BigBalls.Factories
{
    public class HitPlayerStrategy : ICollisionStrategy
    {

        private IEntityRepository _entityRepository;
        private IDamageService _damageService;
        private SoundSpawner _soundSpawner;

        [Inject]
        public void Construct (IEntityRepository entityRepository, IDamageService damageService, SoundSpawner soundSpawner)
        {
            _entityRepository = entityRepository;
            _damageService = damageService;
            _soundSpawner = soundSpawner;
        }

        public bool HandleCollision (Ball ball, Collision collision)
        {
            if (collision.gameObject.TryGetComponent(out Player player))
            {
                OnHit(player, ball);
                ball.EventHandler.Return(ball);

                return true;
            }

            return false;
        }

        public void HandleTrigger (Ball ball, Collider other)
        {
        }

        private void OnHit (Player player, Ball ball)
        {
            StatHolder statHolder = _entityRepository.GetStatHolder(ball);

            if (statHolder.TryGetStat(out Stat stat, StatType.Damage))
            {
                float damage = _damageService.ApplyDamage(ball, player, stat.CurrentValue, ball.Config.DamageColor);
                ball.CalculateDamage(damage);
            }

            _soundSpawner.Spawn(ball.Config.HitSound);
            ball.SetCanReturnToBag(true);
            ball.EventHandler.HitEntity(player);
        }
    }
}