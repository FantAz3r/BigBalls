
public class BootstrapState : IState
{
    private IGameStateMachine _stateMachine;

    public BootstrapState(IGameStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public void Enter()
    {
        _stateMachine.EnterIn<LoadingLevelState, LevelID>(LevelID.MainMenu);
    }

    public void Exit()
    {

    }

    private void RegisterServices()
    {
       
    }
}
