public class LevelLoadingService : ILevelLoadingService
{
    private readonly IGameStateMachine _stateMachine;

    public LevelLoadingService(IGameStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public void Load(LevelID level) => _stateMachine.EnterIn<LoadingLevelState, LevelID>(level);
}
