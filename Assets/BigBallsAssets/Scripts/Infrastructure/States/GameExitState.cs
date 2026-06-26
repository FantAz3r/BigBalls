public class GameExitState : IState
{
    private readonly BallsRepository _ballsRepository;
    private readonly ArtefactsRepository _artefactsRepository;
    private readonly ISaveService _saveService;

    public GameExitState (
        BallsRepository ballsRepository,
        ArtefactsRepository artefactsRepository,
        ISaveService saveService)
    {
        _ballsRepository = ballsRepository;
        _artefactsRepository = artefactsRepository;
        _saveService = saveService;
    }

    public void Enter ()
    {
        _ballsRepository.Save();
        _artefactsRepository.Save();
        _saveService.Save();
    }

    public void Exit ()
    {
    }
}
