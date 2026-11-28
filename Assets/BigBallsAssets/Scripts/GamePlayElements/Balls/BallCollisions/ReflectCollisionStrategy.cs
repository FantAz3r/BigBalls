using BigBalls.GameplayObjects;
using BigBalls.Services;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public class ReflectCollisionStrategy : ICollisionStrategy
{
    private const float ReflectionJitterDegrees = 0.5f;

    private bool _isReflect;
    private bool _isReflectToPlayer;
    private SoundSpawner _soundSpawner;
    private IPlayerProvider _playerProvider;

    public ReflectCollisionStrategy (bool ifReflectToPlayer = false)
    {
        _isReflectToPlayer = ifReflectToPlayer;
    }

    [Inject]
    public void Construct (SoundSpawner soundSpawner, IPlayerProvider playerProvider)
    {
        _soundSpawner = soundSpawner;
        _playerProvider = playerProvider;
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
        if (_isReflectToPlayer)
        {
            if (_playerProvider.Player == null)
                return;

            Vector3 player = _playerProvider.Player.transform.position;
            SetDirection(ball, player);
        }
        else
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

            SetDirection(ball, reflectedDirection);
        }
    }

    private void SetDirection(Ball ball, Vector3 direction)
    {
        ball.Mover.SetDirection(new Vector2(direction.x, direction.z));
        ball.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        ball.EventHandler.Reflect(ball);
    }
}