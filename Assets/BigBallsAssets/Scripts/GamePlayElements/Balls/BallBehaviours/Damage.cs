using BigBalls.Configs;
using BigBalls.Services;
using System;
using UnityEngine;
using VContainer;

namespace BigBalls.GameplayObjects
{
    public class Damage : EffectBehaviour
    {
        private readonly DamageConfig _config;
        private IDamageService _damageService;
        private SoundSpawner _soundSpawner;

        public Damage(DamageConfig config, int level) : base(config, level)
        {
            _config = config;
        }

        [Inject]
        public void Construct(IDamageService damageService, SoundSpawner soundSpawner)
        {
            _damageService = damageService;
            _soundSpawner = soundSpawner;
        }

        private void OnHit(IEntity entity)
        {
            if (Host is Ball ball)
            {
                _soundSpawner.Spawn(ball.Config.HitSound);
                float damage = _damageService.ApplyDamage(entity, _config.GetDamage(Level), ball.Config.DamageColor);
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
