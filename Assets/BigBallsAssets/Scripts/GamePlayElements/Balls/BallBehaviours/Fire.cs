using System;
using System.Collections;
<<<<<<< HEAD
using System.Collections.Generic;
=======
>>>>>>> f0e424ab67e4621452446e379326bfb4b7df25bf
using BigBalls.Configs;
using BigBalls.Factories;
using BigBalls.Services;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace BigBalls.GameplayObjects
{
    public class Fire : EffectBehaviour
    {
        private readonly FireConfig _config;
        private IDamageService _damageService;
        private ICoroutineRunner _coroutineRunner;
        //private IParticleFactory _particleFactory;
        private WaitForSeconds _oneSecond = new WaitForSeconds(1);

        public Fire (FireConfig config, int level) : base(config, level)
        {
            _config = config;
        }

        [Inject]
        public void Construct (IDamageService damageService, ICoroutineRunner coroutineRunner)
        {
            _damageService = damageService;
            _coroutineRunner = coroutineRunner;
            //_particleFactory = particleFactory;
        }

<<<<<<< HEAD
        public float GetBurnDuration(int level)
        {
            return _config.BurnDuration * (1 + level);
        }

=======
>>>>>>> f0e424ab67e4621452446e379326bfb4b7df25bf
        private void OnHit (IEntity entity)
        {
            _coroutineRunner.StartCoroutine(Burn(entity));
        }

        private IEnumerator Burn (IEntity entity)
        {
            var elapsed = 0f;

            while (elapsed <= _config.BurnDuration)
            {
                if (entity.Transform.gameObject.activeSelf == false)
                    yield break;

<<<<<<< HEAD
                float damage = _damageService.ApplyDamage(entity, _config.BurnDPS);
=======
                float damage = _damageService.ApplyDamage(entity, _config.DamagePerSecond);
>>>>>>> f0e424ab67e4621452446e379326bfb4b7df25bf
                elapsed += Time.deltaTime;
                yield return _oneSecond;
            }
        }

        protected override IDisposable SubscribeInternal (IEntity host)
        {
            host.EventHandler.HitedEntity += OnHit;
            return new DisposableObject(() => host.EventHandler.HitedEntity -= OnHit);
        }
    }
}
