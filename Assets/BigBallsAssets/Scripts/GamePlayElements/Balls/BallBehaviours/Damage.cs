using BigBalls.Configs;
using BigBalls.Services;
using System;
using VContainer;

namespace BigBalls.GameplayObjects
{
    public class Damage : EffectBehaviour
    {
        private readonly DamageConfig _config;
        private IDamageService _damageService;

        public Damage(DamageConfig config, int level) : base(config, level)
        {
            _config = config;
        }

        [Inject]
        public void Construct(IDamageService damageService)
        {
            _damageService = damageService;
        }

        private void OnHit(IEntity entity)
        {
            if (Host is Ball ball)
            {
                float damage = _damageService.ApplyDamage(entity, _config.GetDamage(Level));
                ball.AddDamage(damage);
            }
        }

        protected override IDisposable SubscribeInternal(IEntity host)
        {
            host.EventHandler.HitedEntity += OnHit;
            return new DisposableObject(() => host.EventHandler.HitedEntity -= OnHit);
        }
    }
}
