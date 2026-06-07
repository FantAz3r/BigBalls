using BigBalls.GameplayObjects;
using UnityEngine;

public class ReflectCollisionStrategy : ICollisionStrategy
{
    public bool HandleCollision(Ball ball, Collision collision)
    {
        ball.SetCanReturnToBag(true);

        if (ball.IsMaterial)
        {
            ball.ReflectBall(collision);
        }
        else if (collision.gameObject.TryGetComponent<Wall>(out _))
        {
            ball.ReflectBall(collision);
        }

        return false;
    }
}