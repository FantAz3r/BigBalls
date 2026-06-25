public class GameExitState : IState
{
    private readonly BallsRepository _ballsRepository;
    private readonly ArtefactsRepository _artefactsRepository;

    public GameExitState (
        BallsRepository ballsRepository,
        ArtefactsRepository artefactsRepository)
    {
        _ballsRepository = ballsRepository;
        _artefactsRepository = artefactsRepository;
    }

    public void Enter ()
    {
        _ballsRepository.Save();
        _artefactsRepository.Save();
    }

    public void Exit ()
    {
    }
}
