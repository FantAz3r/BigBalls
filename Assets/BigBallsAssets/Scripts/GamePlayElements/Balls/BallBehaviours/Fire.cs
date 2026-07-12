using BigBalls.Configs;
using BigBalls.Services;
using System;
using System.Collections;
using UnityEngine;
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

        public Fire(FireConfig config, int level) : base(config, level)
        {
            _config = config;
        }

        [Inject]
        public void Construct(IDamageService damageService, ICoroutineRunner coroutineRunner)
        {
            _damageService = damageService;
            _coroutineRunner = coroutineRunner;
            //_particleFactory = particleFactory;
        }

        public float GetBurnDuration(int level)
        {
            return _config.BurnDuration * (1 + level);
        }

        private void OnHit(IEntity entity)
        {
            _coroutineRunner.StartCoroutine(Burn(entity));
        }

        private IEnumerator Burn(IEntity entity)
        {
            var elapsed = 0f;

            while (elapsed <= _config.BurnDuration)
            {
                if (entity.Transform.gameObject.activeSelf == false)
                    yield break;

                float damage = _damageService.ApplyDamage(entity, _config.BurnDPS);
                elapsed += Time.deltaTime;
                yield return _oneSecond;
            }
        }

        protected override IDisposable SubscribeInternal(IEntity host)
        {
            host.EventHandler.HitedEntity += OnHit;
            return new DisposableObject(() => host.EventHandler.HitedEntity -= OnHit);
        }
    }
}
