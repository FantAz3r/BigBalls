using System;
using BigBalls.GameplayObjects;
using BigBalls.Services;
using BigBalls.StaticData;
using VContainer;

public class Ice : EffectBehaviour
{
    private readonly IceConfig _config;
    private ICoroutineRunner _coroutineRunner;
    private IEntityRepository _entityRepository;

    public Ice(IceConfig config, int level) : base(config, level)
    {
        _config = config;
    }

    [Inject]
    public void Construct(ICoroutineRunner coroutineRunner, IEntityRepository entityRepository)
    {
        _coroutineRunner = coroutineRunner;
        _entityRepository = entityRepository;
    }

    public void OnHit(IEntity entity)
    {

    }

    protected override IDisposable SubscribeInternal(IEntity host)
    {
        host.EventHandler.HitedEntity += OnHit;
        return new DisposableObject(() => host.EventHandler.HitedEntity -= OnHit);
    }
}
