using BigBalls.GameplayObjects;
using UnityEngine;
using VContainer;

public class ReflectCollisionStrategy : ICollisionStrategy
{
    private const float ReflectionJitterDegrees = 0.5f;

    private bool _isReflect;
    private SoundSpawner _soundSpawner;

    [Inject]
    public void Construct (SoundSpawner soundSpawner = null)
    {
        _soundSpawner = soundSpawner;
    }

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

    public void HandleTrigger (Ball ball, Collider other)
    {
    }

    public void ReflectBall (Collision collision, Ball ball)
    {
        Vector3 currentDirection = new Vector3(ball.Mover.Direction.normalized.x, 0, ball.Mover.Direction.normalized.y);
        Vector3 normal = collision.contacts[0].normal;
        Vector3 reflectedDirection = Vector3.Reflect(currentDirection, normal);
        reflectedDirection.y = 0;
        reflectedDirection.Normalize();

        float jitterAngle = Random.Range(-ReflectionJitterDegrees, ReflectionJitterDegrees);
        reflectedDirection = Quaternion.AngleAxis(jitterAngle, Vector3.up) * reflectedDirection;

        reflectedDirection += normal * 0.02f;
        reflectedDirection.Normalize();

        ball.Mover.SetDirection(new Vector2(reflectedDirection.x, reflectedDirection.z));
        ball.EventHandler.Reflect(ball);
    }
}