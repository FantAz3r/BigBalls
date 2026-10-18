using BigBalls.GameplayObjects;
using BigBalls.Services;
using UnityEngine;
using VContainer;

public abstract class BossStateBase : IEnemyState, IUpdateble
{
    protected float Elapsed;
    protected float Duration;
    private IUpdateService _updateService;
    protected BossStateBase (EnemyStateMachine enemyStateMachine, Enemy enemy, float duration)
    {
        StateMachine = enemyStateMachine;
        Enemy = enemy;
        Duration = duration;
        Debug.Log(Duration);
    }

    public EnemyStateMachine StateMachine { get; private set; }
    public Enemy Enemy { get; private set; }

    [Inject]
    public void Construct (IUpdateService updateService)
    {
        _updateService = updateService;
    }

    public virtual void Enter ()
    {
        _updateService.Register(this);
        Elapsed = 0f;
        OnEnter();
    }

    public virtual void Tick ()
    {
        Elapsed += Time.deltaTime;
        OnTick(Time.deltaTime);

        if (Duration == 0)
            return;

        if (Elapsed >= Duration)
            OnComplete();
    }

    public virtual void Exit () => OnExit();

    protected abstract void OnEnter ();
    protected abstract void OnTick (float dt);
    protected abstract void OnComplete ();
    protected virtual void OnExit ()
    {
        _updateService.Unregister(this);
    }
}