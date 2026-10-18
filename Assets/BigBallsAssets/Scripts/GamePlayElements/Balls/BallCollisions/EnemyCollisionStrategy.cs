using BigBalls.GameplayObjects;
using UnityEngine;

public class EnemyCollisionStrategy : ICollisionStrategy
{
    public bool HandleCollision (Ball ball, Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Shield>(out _))
        {
            return false;
        }

        if (collision.gameObject.TryGetComponent(out Enemy enemy))
        {
            if (enemy == null)
                return false;

            OnHit(enemy, ball);
        }
        else if (collision.gameObject.TryGetComponent(out EnemyPart component))
        {
            if (component.Enemy == null)
                return false;

            OnHit(component.Enemy, ball);
        }

        ball.EventHandler.Hit();
        return false;
    }

    private void OnHit (Enemy enemy, Ball ball)
    {
        ball.SetCanReturnToBag(true);
        ball.EventHandler.HitEntity(enemy);

        if (enemy.EnemyHitedAnimation != null)
        {
            enemy.EnemyHitedAnimation.OnHit(ball);
        }
    }
}
