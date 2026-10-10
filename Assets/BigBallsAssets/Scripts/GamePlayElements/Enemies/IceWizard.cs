using System.Collections.Generic;
using BigBalls.Factories;
using BigBalls.GameplayObjects;
using UnityEngine;

public class IceWizard : Enemy
{
    [SerializeField] private Transform _firePoint;
    protected override void ChildInit ()
    {
        List<ICollisionStrategy> strategies = new List<ICollisionStrategy>()
        {
            new ReflectCollisionStrategy(),
            new ShieldEnemyStrategy(),
            new HitPlayerStrategy(),
            new BallCollisionStrategy()
        };

        Shooter.Set(strategies);
        Shooter.Set(_firePoint);
    }
}
