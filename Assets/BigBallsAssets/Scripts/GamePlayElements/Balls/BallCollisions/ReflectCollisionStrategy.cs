using BigBalls.GameplayObjects;
using UnityEngine;

public class ReflectCollisionStrategy : ICollisionStrategy
{
    public bool HandleCollision(Ball ball, Collision collision)
    {
        ball.SetCanReturnToBag(true);

        if (ball.IsMaterial)
        {
            ReflectBall(collision, ball);
        }
        else if (collision.gameObject.TryGetComponent<Wall>(out _))
        {
            ReflectBall(collision, ball);
        }

        return false;
    }

    public void ReflectBall(Collision collision, Ball ball)
    {
        Vector3 currentDirection = new Vector3(ball.Mover.Direction.normalized.x, 0, ball.Mover.Direction.normalized.y);
        Vector3 normal = collision.contacts[0].normal;
        Vector3 reflectedDirection = Vector3.Reflect(currentDirection, normal);
        ball.Mover.SetDirection(new Vector2(reflectedDirection.x, reflectedDirection.z));
    }
}