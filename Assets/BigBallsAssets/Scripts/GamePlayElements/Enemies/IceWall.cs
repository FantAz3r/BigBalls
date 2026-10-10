using System.Collections.Generic;
using BigBalls.Factories;
using BigBalls.GameplayObjects;
using BigBalls.Services;
using UnityEngine;
using VContainer;

public class IceWall : MonoBehaviour
{
    private const float OffsetY = 0.5f;

    [SerializeField] private Ball _prefab;
    [SerializeField] private float _spreadAngle;
    [SerializeField] private int _projectileCount = 5;

    private IPoolService _poolService;
    private IBallFactory _ballFactory;
    private IIdentifierService _identifierService;

    [Inject]
    public void Construct (
        IPoolService poolService,
        IBallFactory ballFactory,
        IIdentifierService identifierService)
    {
        _poolService = poolService;
        _ballFactory = ballFactory;
        _identifierService = identifierService;
    }

    public void BreakWall ()
    {
        for (int i = 0; i < _projectileCount; i++)
        {
            BallModel ballModel = new BallModel(_identifierService.ID, _prefab.Config);
            Ball ball = _ballFactory.Create(ballModel, EntityType.Enemy, transform, CreateStrategies());

            float angleStep = _projectileCount > 1 ? _spreadAngle / (_projectileCount - 1) : 0;
            float currentAngle = -(_spreadAngle / 2f) + (i * angleStep);

            Quaternion rotation = Quaternion.Euler(0, currentAngle, 0);
            Vector3 direction = transform.rotation * rotation * Vector3.forward;

            ball.transform.position = new Vector3(transform.position.x, OffsetY, transform.position.z);
            Debug.Log(ball.transform.position);
            ball.transform.rotation = rotation * transform.rotation;
            ball.Subscribe();

            ball.Mover.SetDirection(direction);
        }
    }

    private void OnBecameInvisible ()
    {
        _poolService.Release(this);
    }

    private List<ICollisionStrategy> CreateStrategies ()
    {
        return new List<ICollisionStrategy>()
        {
             new HitPlayerStrategy(),
             new ReflectCollisionStrategy(),
             new BallCollisionStrategy()
        };
    }
}