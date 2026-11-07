using BigBalls.GameplayObjects;
using UnityEngine;

public class ShieldEnemyStrategy : ICollisionStrategy
{
    public bool HandleCollision (Ball ball, Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Enemy enemy))
        {
            if (enemy is not IceWizard)
            {
                Stat shield = enemy.StatHolder[StatType.Shield];
                shield.AddCurrentValue(10);
                ball.EventHandler.Return(ball);
            }
        }

        return false;
    }

    public void HandleTrigger (Ball ball, Collider other)
    {
    }
}
