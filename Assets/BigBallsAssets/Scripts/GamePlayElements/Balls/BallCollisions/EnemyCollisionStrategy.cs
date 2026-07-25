using BigBalls.GameplayObjects;
using UnityEngine;

public class EnemyCollisionStrategy : ICollisionStrategy
{
    public bool HandleCollision(Ball ball, Collision collision)
    {
        if (collision.gameObject.GetComponent<Enemy>() is Enemy entity)
        {
            ball.SetCanReturnToBag(true);
            ball.EventHandler.HitEntity(entity);

            if (entity.EnemyHitedAnimation != null)
            {
                entity.EnemyHitedAnimation.OnHit(ball);
            }
        }

        ball.EventHandler.Hit();

        return false;
    }
}
