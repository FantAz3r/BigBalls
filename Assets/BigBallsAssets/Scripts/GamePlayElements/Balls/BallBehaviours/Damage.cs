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

        public Damage(DamageConfig config, int level) : base(config, level)
        {
            _config = config;
        }

        [Inject]
        public void Construct(IDamageService damageService)
        {
            _damageService = damageService;
        }

        private void OnHit(int id)
        {
            _damageService.ApplyDamage(id, _config.GetDamage(Level));
        }

        protected override IDisposable SubscribeInternal(IEntity host)
        {
            host.EventHandler.Hited += OnHit;

            return new DisposableObject(() => host.EventHandler.Hited -= OnHit);
        }
    }
}
