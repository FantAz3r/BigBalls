using BigBalls.GameplayObjects;
using UnityEngine;

public class ReflectCollisionStrategy : ICollisionStrategy
{
    private bool _isReflect;
    private SoundSpawner _soundSpawner;
    public ReflectCollisionStrategy (SoundSpawner soundSpawner = null) => _soundSpawner = soundSpawner;

    public bool HandleCollision (Ball ball, Collision collision)
    {
        if (ball.IsMaterial)
        {
            ReflectBall(collision, ball);
            _isReflect = true;
        }

        if (collision.gameObject.TryGetComponent<Wall>(out _))
        {
            if (_isReflect == false)
            {
                ReflectBall(collision, ball);
            }

            _soundSpawner?.Spawn(SoundType.BallHitWall);
            ball.SetCanReturnToBag(true);
        }

        return false;
    }

    public void ReflectBall (Collision collision, Ball ball)
    {
        Vector3 currentDirection = new Vector3(ball.Mover.Direction.normalized.x, 0, ball.Mover.Direction.normalized.y);
        Vector3 normal = collision.contacts[0].normal;
        Vector3 reflectedDirection = Vector3.Reflect(currentDirection, normal);
        reflectedDirection.y = 0;
        reflectedDirection.Normalize();
        ball.Mover.SetDirection(new Vector2(reflectedDirection.x, reflectedDirection.z));
        ball.EventHandler.Reflect(ball);
    }
}