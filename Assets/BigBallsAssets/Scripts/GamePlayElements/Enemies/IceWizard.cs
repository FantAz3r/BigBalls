using System.Collections.Generic;
using BigBalls.Factories;
using BigBalls.GameplayObjects;
using BigBalls.Infrastructure.DI;
using UnityEngine;
using VContainer;

public class IceWizard : Enemy
{
    [SerializeField] private Transform _firePoint;

    private IObjectResolverProvider _objectResolverProvider;

    [Inject]
    public void Construct(IObjectResolverProvider objectResolverProvider)
    {
        _objectResolverProvider = objectResolverProvider;
    }

    protected override void ChildInit ()
    {
        List<ICollisionStrategy> strategies = new List<ICollisionStrategy>()
        {
            new ReflectCollisionStrategy(),
            new ShieldEnemyStrategy(),
            new HitPlayerStrategy(),
            new BallCollisionStrategy()
        };

        foreach (ICollisionStrategy strategy in strategies)
            _objectResolverProvider.CurrentResolver.Inject(strategy);

        Shooter.Set(strategies);
        Shooter.Set(_firePoint);
    }
}
