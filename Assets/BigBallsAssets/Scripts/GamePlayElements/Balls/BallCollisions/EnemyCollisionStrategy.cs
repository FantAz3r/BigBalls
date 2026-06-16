using BigBalls.GameplayObjects;
using UnityEngine;

public class EnemyCollisionStrategy : ICollisionStrategy
{
    public bool HandleCollision(Ball ball, Collision collision)
    {
        if (collision.gameObject.GetComponentInParent<IEntity>() is IEntity entity)
        {
            ball.SetCanReturnToBag(true);
            ball.EventHandler.Hit(entity.Id);
        }

        return false;
    }
}
