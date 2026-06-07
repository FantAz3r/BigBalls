using BigBalls.GameplayObjects;
using UnityEngine;

public class EnemyCollisionStrategy : ICollisionStrategy
{
    public bool HandleCollision(Ball ball, Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out IEntity entity))
        {
            ball.SetCanReturnToBag(true);
            ball.OnHit(entity.Id);
        }

        return false;
    }
}
