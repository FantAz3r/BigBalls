using System;
using System.Collections.Generic;
using BigBalls.Infrastructure.DI;
using VContainer;

public class EnemyStateMachine
{
    private readonly Dictionary<Type, IEnemyState> _states = new();
    private IEnemyState _current;
    private IObjectResolver _objectResolver;

    public EnemyStateMachine (IObjectResolverProvider objectResolverProvider)
    {
        _objectResolver = objectResolverProvider.CurrentResolver;
    }

    public IEnemyState Current => _current;

    public void Register<T> (T state) where T : IEnemyState
    {
        _states[typeof(T)] = state;
        _objectResolver.Inject(state);
    }

    public void Start<T> () where T : IEnemyState => ChangeState<T>();

    public void Stop ()
    {
        _current?.Exit();
        _current = null;
    }

    public void ChangeState<T> () where T : IEnemyState
    {
        if (_states.TryGetValue(typeof(T), out var next) == false)
            return;

        ChangeState(next);
    }

    public void ChangeState (IEnemyState state)
    {
        if (state == null) 
            return;

        if (ReferenceEquals(_current, state))
            return;

        _current?.Exit();
        _current = state;
        _current.Enter();
    }
}