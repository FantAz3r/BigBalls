using BigBalls.Infrastructure.DI;
using VContainer;

namespace BigBalls.Infrastructure
{
    public class GameStateMachine : IGameStateMachine
    {
        private readonly IObjectResolverProvider _objectResolverProvider;
        private IExitableState _currentState;

        public GameStateMachine(IObjectResolverProvider objectResolverProvider)
        {
            _objectResolverProvider = objectResolverProvider;
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

            TState state = _objectResolverProvider.CurrentResolver.Resolve<TState>();
            _currentState = state;
            return state;
        }
    }
}

