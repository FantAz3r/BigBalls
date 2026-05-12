public interface IGameStateMachine
{
    void EnterIn<TState, TPayload>(TPayload levelID) where TState : class, IPayloadedState<TPayload>;
    void EnterIn<TState>() where TState : class, IState;
}