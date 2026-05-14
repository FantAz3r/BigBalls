using System;
using System.Collections.Generic;
using UnityEngine;

public class GameStateMachine : IGameStateMachine
{
    private Dictionary<Type, IExitableState> _states;
    private IExitableState _currentState;

    public GameStateMachine(ISceneLoader sceneLoader)
    {
        Debug.Log("GameStateMachine");
        _states = new Dictionary<Type, IExitableState>();
        _states[typeof(BootstrapState)] = new BootstrapState(this);
        _states[typeof(LoadingLevelState)] = new LoadingLevelState(sceneLoader);
        _states[typeof(PersistentProgressState)] = new PersistentProgressState();
    }

    public void EnterIn<TState, TPayload>(TPayload levelID) where TState : class, IPayloadedState<TPayload>
    {
        TState state = ChangeState<TState>();
        state.Enter(levelID);
    }

    public void EnterIn<TState>() where TState : class, IState
    {
        TState state = ChangeState<TState>();
        state.Enter();
    }

    private TState ChangeState<TState>() where TState : class, IExitableState
    {
        if (_currentState is IExitableState exitableState)
            exitableState.Exit();

        TState state = GetState<TState>();
        _currentState = state;
        return state;
    }

    private TState GetState<TState>() where TState : class, IExitableState => _states[typeof(TState)] as TState;
}

