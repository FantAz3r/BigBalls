using System;
using BigBalls.Configs;
using BigBalls.Services;
using VContainer;

namespace BigBalls.GameplayObjects
{
    public class Damage : EffectBehaviour
    {
        private readonly DamageConfig _config;
        private IDamageService _damageService;
        private SoundSpawner _soundSpawner;
        private IEntityRepository _entityRepository;

        public Damage (DamageConfig config, int level) : base(config, level)
        {
            _config = config;
        }

        [Inject]
        public void Construct (IDamageService damageService, SoundSpawner soundSpawner, IEntityRepository entityRepository)
        {
            _damageService = damageService;
            _soundSpawner = soundSpawner;
            _entityRepository = entityRepository;
        }

        protected override IDisposable SubscribeInternal (IEntity host)
        {
            host.EventHandler.HitedEntity += OnHit;
            StatHolder statHolder = _entityRepository.GetStatHolder(host);

            if (statHolder.TryGetStat(out Stat damage, StatType.Damage))
            {
                damage.ReduceCurrentValue(damage.CurrentValue);
                damage.AddCurrentValue(_config.GetDamage(Level));
            }

            return new DisposableObject(() => host.EventHandler.HitedEntity -= OnHit);
        }

        private void OnHit (IEntity host)
        {
            if (Host is not Ball ball)
                return;

            if (ball.Config.IsPlayerBall)
                _soundSpawner.Spawn(ball.Config.HitSound);
        }
    }
}
