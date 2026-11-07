using System;
using System.Collections.Generic;
using BigBalls.Factories;
using BigBalls.GameplayObjects;
using BigBalls.Infrastructure.DI;
using UnityEngine;
using VContainer;

public class Split : EffectBehaviour
{
    private readonly SplitConfig _config;
    private IBallFactory _ballFactory;
    private int _reflectCount;
    private BallRepository _ballRepository;
    private IObjectResolverProvider _objectResolverProvider;

    public Split (SplitConfig config, int level) : base(config, level)
    {
        _config = config;
    }

    [Inject]
    public void Construct (IBallFactory ballsFactory, BallRepository ballsRepository, IObjectResolverProvider objectResolverProvider)
    {
        _ballFactory = ballsFactory;
        _ballRepository = ballsRepository;
        _objectResolverProvider = objectResolverProvider;
    }

    protected override IDisposable SubscribeInternal (IEntity host)
    {
        host.EventHandler.Reflected += OnReflect;
        return new DisposableObject(() => host.EventHandler.Reflected -= OnReflect);
    }

    private void OnReflect (IEntity entity)
    {
        _reflectCount++;

        int hitsPerSplit = (int)_config.GetHitsPerSplit(Level);

        if (_reflectCount < hitsPerSplit) return;

        _reflectCount = 0;
        OnSplit((int) _config.GetBallsPerSplit(Level), entity);
    }

    private void OnSplit (int count, IEntity host)
    {
        if (count <= 0)
            return;

        if (host is not Ball origin)
            return;

        Vector2 baseDir = origin.Mover != null
            ? origin.Mover.Direction
            : Vector2.up;

        float baseAngle = Mathf.Atan2(baseDir.y, baseDir.x) * Mathf.Rad2Deg;
        float step = 360f / count;

        for (int i = 0; i < count; i++)
        {
            float angle = baseAngle + i * step;

            float rad = angle * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

            var childModel = _ballRepository.AllModels[_config.ChildType];

            var strategies = CreateCollisionStrategies();

            Vector3 pos = origin.transform.position;
            Quaternion rot = Quaternion.LookRotation(
                new Vector3(dir.x, 0f, dir.y), Vector3.up);

            var ball = _ballFactory.Create(
                childModel,
                EntityType.None,
                origin.transform,
                strategies,
                rot);

            ball.transform.position = pos;
            ball.transform.rotation = rot;
            ball.Mover.SetDirection(dir);
        }
    }

    private List<ICollisionStrategy> CreateCollisionStrategies ()
    {
        List<ICollisionStrategy> strategies = new List<ICollisionStrategy>()
        {
            new BallCollisionStrategy(),
            new PlayerCollisionStrategy(),
            new BackWallCollisionStrategy(),
            new EnemyCollisionStrategy(),
            new ReflectCollisionStrategy()
        };

        foreach (var strategy in strategies)
            _objectResolverProvider.CurrentResolver.Inject(strategy);

        return strategies;
    }
}