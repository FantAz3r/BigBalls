using BigBalls.Factories;
using BigBalls.GameplayObjects;
using BigBalls.Services;
using System.Collections.Generic;

public class BallBehaivorFactory : IBallBehaivorFactory
{
    private readonly IDamageService _damageService;

    private Dictionary<BehaviourType, Behaviour> _ballBehaivors = new();

    public BallBehaivorFactory(IDamageService damageService)
    {
        _damageService = damageService;
        _ballBehaivors = new Dictionary<BehaviourType, Behaviour>()
        {
            { BehaviourType.Damage, new DamageBehaivor(_damageService) },
        };
    }

    public Behaviour Get(BehaviourType type) => _ballBehaivors[type];
}
