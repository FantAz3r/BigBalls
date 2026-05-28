using BigBalls.Factories;
using BigBalls.Services;
using System.Collections.Generic;

namespace BigBalls.GameplayObjects
{
    public class BallBehaivorFactory : IBallBehaivorFactory
    {
        private readonly IDamageService _damageService;

        private Dictionary<BehaviourType, Behaviour> _ballBehaivors = new();

        public BallBehaivorFactory(IDamageService damageService)
        {
            _damageService = damageService;

            _ballBehaivors = new Dictionary<BehaviourType, Behaviour>()
            {
                {BehaviourType.Damage, new DamageBehaivor(BehaviourType.Damage, _damageService) },
            };
        }

        public Behaviour Get(BehaviourType type) => _ballBehaivors[type];
    }
}
