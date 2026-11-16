using BigBalls.GameplayObjects;
using UnityEngine;

public class HitTeammateStrategy : ICollisionStrategy
{
    public bool HandleCollision (Ball ball, Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Enemy enemy))
        {
            if (enemy == null)
                return false;

            ball.EventHandler.HitEntity(enemy);
            return false;
        }

        return false;
    }

    public void HandleTrigger (Ball ball, Collider other)
    {
    }
}
