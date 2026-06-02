using BigBalls.Services;

namespace BigBalls.GameplayObjects
{
    public class DamageBehaivor : Behaviour
    {
        private readonly IDamageService _damageService;

        public DamageBehaivor(BehaviourType type, IDamageService damageService) : base(type) 
        {
            _damageService = damageService;
        }

        protected override void OnInit()
        {
            Host.Hited += OnHit;
        }

        private void OnHit(int id)
        {
            _damageService.ApplyDamage(id, Host.Damage);
        }
    }
}