using BigBalls.Services;
using BigBalls.StaticData;
using VContainer;

namespace BigBalls.GameplayObjects
{
    public class Damage : EffectBehaviour
    {
        private readonly DamageBehaviour _config;
        private IDamageService _damageService;

        public Damage(DamageBehaviour config) : base(config)
        {
            _config = config;
        }

        [Inject]
        public void Construct(IDamageService damageService)
        {
            _damageService = damageService;

        }

        protected override void OnHit(int id)
        {
            _damageService.ApplyDamage(id, _config.Damage);
        }
    }
}
