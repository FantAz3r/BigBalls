using BigBalls.Factories;
using BigBalls.Services;
using BigBalls.StaticData;
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
        private IParticleFactory _particleFactory;
        private WaitForSeconds _oneSecond = new WaitForSeconds(1);

        public Fire(FireConfig config) : base(config)
        {
            _config = config;
        }

        [Inject]
        public void Construct(IDamageService damageService, IParticleFactory particleFactory, ICoroutineRunner coroutineRunner)
        {
            _damageService = damageService;
            _coroutineRunner = coroutineRunner;
            _particleFactory = particleFactory;
        }

        protected override void OnHit(int id)
        {
            _coroutineRunner.StartCoroutine(Burn(id));
        }

        private IEnumerator Burn(int targetId)
        {
            var elapsed = 0f;

            while (elapsed < _config.BurnDuration)
            {
                _damageService.ApplyDamage(targetId, _config.DamagePerSecond);
                elapsed += Time.deltaTime;
                yield return _oneSecond;
            }
        }
    }
}
